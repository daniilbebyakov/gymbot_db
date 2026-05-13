using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace GymBot.Data.Data.Repositories
{
    public class WorkoutRepository(GymBotContext context)
    {
        private readonly GymBotContext _context = context;

        public sealed record WorkoutExerciseDto(string Name, decimal Weight, int Reps, int Sets);
        public sealed record WorkoutHistoryItemDto(long Id, string Name, DateOnly Date, int ExercisesCount);
        public async Task<long> SaveWorkout(long tgId, DateOnly workoutDate, string workoutTemplate,
            IReadOnlyCollection<WorkoutExerciseDto> exercises)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TgId == tgId)
                ?? throw new InvalidOperationException("Пользователь не зарегистрирован");
            var workout = new Workout
            {
                Name = workoutTemplate.Trim(),
                UserId = user.Id,
                Date = workoutDate,
            };
            foreach (var exerciseDto in exercises)
            {
                var exercise = new Exercise
                {
                    Name = exerciseDto.Name.Trim(),
                    Date = workoutDate,
                };

                for (var setNumber = 0; setNumber < exerciseDto.Sets; setNumber++)
                {
                    exercise.Sets.Add(new SetEntry
                    {
                        Reps = exerciseDto.Reps,
                        Weight = exerciseDto.Weight,
                        Date = workoutDate,
                    });
                }
                workout.Exercises.Add(exercise);
            }
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout.Id;
        }
        public async Task<IReadOnlyCollection<WorkoutHistoryItemDto>> GetWorkoutHistory(long tgId, int page, int pageSize = 5)
        {
            return await _context.Workouts
                .AsNoTracking()
                .Where(w => w.User.TgId == tgId)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(w => new WorkoutHistoryItemDto(
                    w.Id,
                    w.Name,
                    w.Date,
                    w.Exercises.Count))
                .ToListAsync();
        }
        public async Task<int> GetWorkoutHistoryCount(long tgId)
        {
            return await _context.Workouts.CountAsync(w => w.User.TgId == tgId);
        }
        public async Task<bool> DeleteWorkout (long tgId, long workoutId)
        {
            var workout=await _context.Workouts.Include(w=>w.User)
                .FirstOrDefaultAsync(w=>w.Id == workoutId && w.User.TgId==tgId);
            if (workout is null)
            {
                return false;
            }
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<byte[]> BuildWeightProgressReport(long tgId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TgId == tgId)
                ?? throw new InvalidOperationException("Пользователь не зарегистрирован");

            var setProgress = await _context.Sets
                .AsNoTracking()
                .Where(s => s.Exercise.Workout.UserId == user.Id)
                .Select(s => new
                {
                    ExerciseName = s.Exercise.Name,
                    s.Date,
                    s.Weight,
                })
                .ToListAsync();

            ExcelPackage.License.SetNonCommercialPersonal("Danya");//xz

            using var package = new ExcelPackage();
            var overviewSheet = package.Workbook.Worksheets.Add("Общий прогресс");
            overviewSheet.Cells[1, 1].Value = "Дата";
            overviewSheet.Cells[1, 2].Value = "Максимальный вес, кг";

            var overviewRows = setProgress
                .GroupBy(x => x.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    MaxWeight = g.Max(x => x.Weight),
                })
                .OrderBy(x => x.Date)
                .ToList();

            for (var i = 0; i < overviewRows.Count; i++)
            {
                var rowNumber = i + 2;
                overviewSheet.Cells[rowNumber, 1].Value = overviewRows[i].Date.ToDateTime(TimeOnly.MinValue);
                overviewSheet.Cells[rowNumber, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                overviewSheet.Cells[rowNumber, 2].Value = overviewRows[i].MaxWeight;
            }

            AddLineChart(overviewSheet, 2, "График веса за весь период", overviewRows.Count + 1);

            var exercisesRows = setProgress
                .GroupBy(x => x.ExerciseName)
                .OrderBy(g => g.Key);

            foreach (var exerciseGroup in exercisesRows)
            {
                var safeName = GetSafeWorksheetName(exerciseGroup.Key);
                var worksheet = package.Workbook.Worksheets.Add(safeName);
                worksheet.Cells[1, 1].Value = "Дата";
                worksheet.Cells[1, 2].Value = "Максимальный вес, кг";

                var rows = exerciseGroup
                    .GroupBy(x => x.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        MaxWeight = g.Max(x => x.Weight),
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                for (var i = 0; i < rows.Count; i++)
                {
                    var rowNumber = i + 2;
                    worksheet.Cells[rowNumber, 1].Value = rows[i].Date.ToDateTime(TimeOnly.MinValue);
                    worksheet.Cells[rowNumber, 1].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[rowNumber, 2].Value = rows[i].MaxWeight;
                }

                AddLineChart(worksheet, 2, $"Прогресс: {exerciseGroup.Key}", rows.Count + 1);
                worksheet.Cells.AutoFitColumns();
            }

            overviewSheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private static void AddLineChart(ExcelWorksheet worksheet, int startColumn, string title, int lastDataRow)
        {
            if (lastDataRow <= 2)
            {
                return;
            }

            var chart = worksheet.Drawings.AddChart($"chart_{worksheet.Name}", eChartType.LineMarkers);
            chart.Title.Text = title;
            chart.SetPosition(1, 0, 3, 0);
            chart.SetSize(900, 450);

            var values = worksheet.Cells[2, 2, lastDataRow, 2];
            var labels = worksheet.Cells[2, 1, lastDataRow, 1];
            var series = chart.Series.Add(values, labels);
            series.Header = "Вес";
        }

        private static string GetSafeWorksheetName(string exerciseName)
        {
            var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            var cleanedName = exerciseName.Trim();

            foreach (var invalidChar in invalidChars)
            {
                cleanedName = cleanedName.Replace(invalidChar, '_');
            }

            if (string.IsNullOrWhiteSpace(cleanedName))
            {
                cleanedName = "Упражнение";
            }

            return cleanedName.Length > 31 ? cleanedName[..31] : cleanedName;
        }
    }
}


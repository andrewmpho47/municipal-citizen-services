namespace MunicipalReporter.Services
{
    public static class ReportingProgressCalculator
    {
        public static int CalculateProgress(bool hasLocation, bool hasCategory, bool hasDescription, bool hasAttachment)
        {
            int[] completedSteps = new int[]
            {
                hasLocation ? 1 : 0,
                hasCategory ? 1 : 0,
                hasDescription ? 1 : 0,
                hasAttachment ? 1 : 0
            };

            int completedCount = CountCompletedRecursive(completedSteps, 0);
            int totalByPointer = SumWithPointer(completedSteps);
            int percentage = (completedCount * 100) / completedSteps.Length;

            return totalByPointer == completedCount ? percentage : 0;
        }

        private static int CountCompletedRecursive(int[] steps, int index)
        {
            if (steps == null || index >= steps.Length)
            {
                return 0;
            }

            return steps[index] + CountCompletedRecursive(steps, index + 1);
        }

        private unsafe static int SumWithPointer(int[] values)
        {
            if (values == null || values.Length == 0)
            {
                return 0;
            }

            int sum = 0;

            fixed (int* start = values)
            {
                int* current = start;

                for (int i = 0; i < values.Length; i++)
                {
                    sum += *(current + i);
                }
            }

            return sum;
        }
    }
}

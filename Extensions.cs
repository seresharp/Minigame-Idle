namespace MinigameIdle
{
    public static class Extensions
    {
        public static int Count<T>(this T[,] self, Func<T, bool> predicate)
        {
            int num = 0;
            for (int i = 0; i < self.GetLength(0); i++)
            {
                for (int j = 0; j < self.GetLength(1); j++)
                {
                    if (predicate(self[i, j]))
                    {
                        num++;
                    }
                }
            }

            return num;
        }
    }
}

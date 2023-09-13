namespace ExpansionEnabler
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Replaces all occurrences of a sequence of bytes with another
		/// sequence of bytes.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="pattern">The byte sequence to replace.</param>
		/// <param name="replacement">The byte sequence to replace with.</param>
		/// <exception cref="ArgumentException">Thrown if pattern and
		/// replacement are not the same size.</exception>
		public static void ReplaceAll(this byte[] src, byte[] pattern, byte[] replacement)
		{
			if (pattern.Length != replacement.Length)
				throw new ArgumentException("Pattern and replacement must be the same size.");

			int[] matches = FindAllMatches(src, pattern);

			foreach (int match in matches)
				src.Insert(replacement, match);
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/283456/13798212
		// Asked by Anders R https://stackoverflow.com/users/36504/anders-r
		// Answered by Ing. Gerardo Sánchez https://stackoverflow.com/users/4685116/ing-gerardo-s%c3%a1nchez
		// Edited by Elmue https://stackoverflow.com/users/1487529/elmue
		/// <summary>
		/// Get all occurrences of a sequence of bytes in the array.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="pattern">The sequence of bytes to look for.</param>
		/// <returns>An array containing the offset for each occurrence.</returns>
		private static int[] FindAllMatches(this byte[] src, byte[] pattern)
		{
			List<int> results = new List<int>();

			int maxFirstCharSlot = src.Length - pattern.Length + 1;
			for (int i = 0; i < maxFirstCharSlot; i++)
			{
				// Compare the first byte.
				if (src[i] != pattern[0])
					continue;

				// Match the rest of the pattern if the first byte matches.
				for (int j = pattern.Length - 1; j >= 1; j--)
				{
					if (src[i + j] != pattern[j])
						break;
					if (j == 1)
						results.Add(i);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// Insert a sequence of bytes into an array at a specific offset.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="insertion">The sequence of bytes to insert.</param>
		/// <param name="offset">The index where the insertion should start.</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if the insertion does
		/// not fit in the array with the provided offset.</exception>
		private static void Insert(this byte[] src, byte[] insertion, int offset)
		{
			for (int i = 0; i < insertion.Length; i++)
			{
				src[offset + i] = insertion[i];
			}
		}
	}
}

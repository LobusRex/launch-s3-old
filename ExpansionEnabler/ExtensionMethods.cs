using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionEnabler
{
	public static class ExtensionMethods
	{
		public static void ReplaceAll(this byte[] src, byte[] pattern, byte[] replacement)
		{
			int[] matches = FindAllMatches(src, pattern);

			foreach (int match in matches)
				src.Insert(replacement, match);
		}

		// Inspired by a question and answer from Stack Overflow.
		// https://stackoverflow.com/q/283456/13798212
		// Asked by Anders R https://stackoverflow.com/users/36504/anders-r
		// Answered by Ing. Gerardo Sánchez https://stackoverflow.com/users/4685116/ing-gerardo-s%c3%a1nchez
		// Edited by Elmue https://stackoverflow.com/users/1487529/elmue
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

		private static void Insert(this byte[] src, byte[] insertion, int offset)
		{
			for (int i = 0; i < insertion.Length; i++)
			{
				src[offset + i] = insertion[i];
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public class SongSeparator
    {
        public static List<string> SeparateSongByLines(string lyrics, int linesPerSlide)
        {
            List<string> slides = new List<string>();
            if (lyrics == null)
                return slides;
            using (StringReader reader = new StringReader(lyrics))
            {
                string line;
                int k = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (k == 0 || k == linesPerSlide)
                    {
                        slides.Add(line);
                        k = 1;
                    }
                    else if (k < linesPerSlide)
                    {
                        slides[slides.Count - 1] += "\r\n" + line;
                        k++;
                    }
                }
            }
            return slides;
        }

        public static List<string> SeparateSongBySymbol(string lyrics, string symbol)
        {
            if (lyrics == null)
                return new List<string> { };
            // Split on 2 or more newline characters (optionally mixed \r\n or \n)
            var slides = Regex.Split(lyrics, symbol);
            slides = Array.FindAll(slides, part => !string.IsNullOrWhiteSpace(part));
            return slides.ToList();
        }
    }
}

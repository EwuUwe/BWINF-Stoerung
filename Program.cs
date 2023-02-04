using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

public class Stoerung
{
    private static readonly char[] satzzeichen = new char[] { '.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '"', '-', '_', '«', '»', '"' };

    public static void Main(string[] _args)
    {
        if(_args.Length!=2)
        {
            Console.WriteLine("Usage: ./Störung-1 [Path to gap text] [Path to Alice in Wonderland]");
            return;
        }

        List<string> words = new List<string>();

        //Read File containing the sentence to be searched for and split it into words
        string[] matchWords = File.ReadAllText(_args[0]).Split(' ');

        //Read File "Alice in Wonderland" and loop through each line
        foreach (var line in File.ReadLines(_args[1]))
        {
            //Clean up line endings
            var correctedLine = line.ReplaceLineEndings("");
            correctedLine = correctedLine.Replace("\n", "");

            words.Add("");
            //Split words and add them to the words list
            for (int i = 0; i < correctedLine.Length; i++)
            {
                //If the current character is a space, add a new empty string to the words list, if it is not a space
                if (correctedLine[i] == ' ')
                {
                    if (words[^1] != "")
                    {
                        words.Add("");
                    }
                }
                //If the current character is punctuation.
                //If last word is empty: set is to the current character, if not, add a new word with the new character
                else if (satzzeichen.Contains(correctedLine[i]))
                {
                    if (words[^1] == "")
                    {
                       words[^1] = correctedLine[i].ToString();
                    }
                    else
                    {
                        words.Add(correctedLine[i].ToString());
                    }

                    words.Add("");
                }
                //If the current character is a normal letter, add it to the last word in the words list
                else
                {
                    words[^1] += correctedLine[i];
                }
            }

            //After going through each character in the current line, check if the last word is empty and remove it if it is
            //This could happen if the last character is punctuation
            if (words[^1] == "")
            {
                words.RemoveAt(words.Count - 1);
            }

            //Add a line break to the words list.
            //This is needed later, when the the words list is searched for matchWords, to know in which line the match was found
            words.Add("\n");
        }

        int startLine = 0; // The line where the first word of matchWords was found
        int lineNumber = 0; // The current line number
        int currentWordToCompare = 0; // The current word in matchWords that is being compared to the words list
        for (var i = 0; i < words.Count; i++)
        {
            var m = words[i];
            if (m == "\n")
            {
                lineNumber++;
                continue;
            }

            //If the current word matches the current word in matchWords, increase currentWordToCompare
            if (m.ToLower() == matchWords[currentWordToCompare] || matchWords[currentWordToCompare] == "_")
            {
                if (currentWordToCompare == 0)
                {
                    startLine = lineNumber;
                }

                //If the current word is the last word in matchWords, print the line where the match was found and reset currentWordToCompare
                if (matchWords.Length - 1 == currentWordToCompare)
                {
                    Console.WriteLine($"Zeile {(startLine + 1)} - Zeile {(lineNumber + 1)}");
                    currentWordToCompare = 0;
                }
                else
                {
                    currentWordToCompare++;
                }
            }
            //If the current word does not match the current word in matchWords, reset currentWordToCompare
            else {
                if(currentWordToCompare != 0 )
                {
                    currentWordToCompare = 0;
                    //Decrease i by 1, so the first word of matchWords is compared to the current word again
                    i--;
                }
            }
        }
    }
}

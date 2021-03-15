using System;
using System.Collections.Generic;

namespace AirlyAPI.Utilities
{
    public static class ArrayUtil
    {
        public static void ArrayPush<T>(ref T[] table, object value)
        {
            Array.Resize(ref table, table.Length + 1); // Resizing the array for the cloned length (+-) (+1-1+1)
            table.SetValue(value, table.Length - 1);
        }

        public static void RemoveArrayValue<T>(ref T[] table, int index)
        {
            // Clone from start index of the element and then pause
            // Then clone from the index + 1 and add this to the final array (the final array copy must start from the provided index)

            T[] removedArray = new T[table.Length - 1];

            int lastIndex = index + 1;
            if (lastIndex > (table.Length - 1))
            {
                Array.Resize(ref table, index);
                return;
            }

            int calced = table.Length - lastIndex;

            Array.ConstrainedCopy(table, 0, removedArray, 0, index <= 0 ? 1 : index); // Copying to the index
            Array.ConstrainedCopy(table, lastIndex, removedArray, index, calced <= 0 ? 1 : calced); // Copying after the specfied index
            table = removedArray;
        }

        public static string JoinList<T>(List<T> list, string character) => JoinArray(list.ToArray(), character);
        public static string JoinArray<T>(T[] table, string character)
        {
            string compiledString = string.Empty;
            foreach (var item in table) compiledString += string.Format("{0}{1}", item, character);

            return compiledString.Remove(compiledString.Length - 1, 1);
        }

        // Splitting the blank characters in a string (CS1011)
        public static List<string> SplitEveryToList(string str) => new List<string>(SplitEvery(str));
        public static string[] SplitEvery(string str)
        {
            string[] array = new string[0];
            foreach (var s in str) ArrayPush(ref array, s);
            return array;
        }

        public static T[] AssignArray<T>(T[] source, T[] target)
        {
            T[] assignedArray = (T[])target.Clone();

            int calculatedLength = target.Length + source.Length;
            int oldLength = target.Length;

            Array.Resize(ref assignedArray, calculatedLength);
            Array.ConstrainedCopy(source, 0, assignedArray, oldLength, source.Length);

            return assignedArray;
        }

        public static T[] RemoveNullValues<T>(T[] table)
        {
            T[] newTabel = (T[]) table.Clone();
            foreach (var element in newTabel)
            {
                int index = Array.IndexOf(newTabel, element);
                if (element == null) RemoveArrayValue(ref newTabel, index);
                else continue;
            }

            return newTabel;
        }
    }
}

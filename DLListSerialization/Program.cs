using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DLListSerialization
{
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand;
        public string   Data;

    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int      Count;

        /* O(nlog(n)) */
        public void Serialize(FileStream s)
        {
            const int _nullIndex = -1;

            var map              = new Dictionary<ListNode, int>();

            /* O(n) */
            var currentNode      = Head;
            for (int i = 0; i < Count; i++)
            {
                map[currentNode] = i;
                currentNode      = Head.Next;
            }

            currentNode = Head;
            AddText(s, "{");
            for (int i = 0; i < Count; i++) /* O(n) */
            {
                AddText(s, "{");

                /* map[item] is O(log(n)) */
                AddText(s, $"{currentNode.Data}," +
                           $"{(currentNode.Rand != null ? map[currentNode.Rand] : _nullIndex)}"
                        );

                AddText(s, "}");
            }
            AddText(s, "}");

            s.Close();
        }

        public void Deserialize(FileStream s)
        {
            const int _nullIndex = -1;

            var bytes            = new byte[s.Length];
            s.Read(bytes, 0, (int)s.Length);
            var elementsStr      = Encoding.UTF8.GetString(bytes).Split(new string[] { "{", "}" }, System.StringSplitOptions.RemoveEmptyEntries);

            var map              = new Dictionary<int, ListNode>();

            var count            = elementsStr.Length;
            ListNode prevNode    = null;
            for (int i = 0; i < count; i++) /* O(n) */
            {
                var variables     = elementsStr[i].Split(',');

                var newNode       = new ListNode();
                newNode.Data      = variables[0];
                if (prevNode != null)
                {
                    newNode.Prev  = prevNode;
                    prevNode.Next = newNode;
                }
                prevNode          = newNode;

                map[int.Parse(variables[1])] = newNode;

                if (i == 0) Head = newNode;
                else if (i == count - 1)
                {
                    Tail         = newNode;
                    Count        = count;
                }
            }

            /* O(n) */
            foreach (var element in map)
            {
                /* map[item] is O(log(n)) */
                element.Value.Rand = element.Key != _nullIndex ? map[element.Key] : null;
            }

            s.Close();
        }

        private void AddText(FileStream fs, string value)
        {
            byte[] buffer = new UTF8Encoding(true).GetBytes(value);
            fs.Write(buffer, 0, buffer.Length);
        }
    }

    class Program
    {
        private const string FileName = "File.txt";
        static void Main(string[] args)
        {
            string pathFile    = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FileName;

            var ListRand       = new ListRand();
            ListRand.Head      = new ListNode();
            ListRand.Head.Data = "s";
            ListRand.Count++;
            ListRand.Serialize(File.Create(pathFile));
            ListRand.Deserialize(File.OpenRead(pathFile));
        }
    }
}

using System.Text;

namespace Maxim
{
	internal class Program
	{
		static HttpClient client = new HttpClient();

		static async Task Main(string[] args)
		{
			var vowels = new HashSet<char>(new char[] { 'a', 'e', 'i', 'o', 'u', 'y' });
			var longestVowelsStringStart = -1;
			var longestVowelsStringEnd = -1;

			var input = Console.ReadLine();

			var badSymbols = IsLowerAscii(input);

			if (badSymbols.Count > 0)
			{
				Console.WriteLine($"Были введены неподходящие символы: {string.Join("", badSymbols)}");
			}
			else
			{
				var symbolCount = new Dictionary<char, int>();

				var result = input.Length % 2 == 0
					? string.Concat(Reverse(input[..(input.Length / 2)]), Reverse(input[(input.Length / 2)..]))
					: string.Concat(Reverse(input), input);

				for (var i = 0; i < result.Length; i++)
				{
					var symbol = result[i];

					if (symbolCount.ContainsKey(symbol))
					{
						symbolCount[symbol]++;
					}
					else
					{
						symbolCount[symbol] = 1;
					}

					if (vowels.Contains(symbol))
					{
						if (longestVowelsStringStart == -1)
						{
							longestVowelsStringStart = i;
						}
						longestVowelsStringEnd = i;
					}
				}

				Console.WriteLine($"Обработанная строка: {result}");

				WriteDictionary(symbolCount);

				Console.WriteLine($"Самая длинная подстрока начинающаяся и заканчивающаяся на гласную: {((longestVowelsStringStart != -1) ? result.Substring(longestVowelsStringStart, longestVowelsStringEnd - longestVowelsStringStart + 1) : "ОТСУТСТВУЕТ")}");

				ChooseSortingAlgorithm(result);

				await RemoveRandomSymbol(result);
			}
		}

		static string Reverse(string input)
		{
			var sb = new StringBuilder(input.Length);

			for (int i = input.Length - 1; i > -1; i--)
			{
				sb.Append(input[i]);
			}

			return sb.ToString();
		}

		static List<char> IsLowerAscii(string input)
		{
			var result = new List<char>();

			foreach (char c in input)
			{
				if ('a' > c || c > 'z')
				{
					result.Add(c);
				}
			}

			return result;
		}

		static void WriteDictionary(Dictionary<char, int> dict)
		{
			Console.WriteLine("Количество входящих символов и их количество:");
			foreach (var item in dict)
			{
				Console.WriteLine($"{item.Key} - {item.Value}");
			}
		}

		static void ChooseSortingAlgorithm(string input)
		{
			var symbols = input.ToCharArray();
			while (true)
			{
				Console.WriteLine("Выберите метод сортировки: 1 - Quicksort, 2 - Tree sort");

				switch (Console.ReadLine())
				{
					case "1":
						QuickSort(symbols, 0, input.Length - 1);
						Console.WriteLine($"Результат QuickSort: {new string(symbols)}");
						return;
					case "2":
						TreeSort(symbols);
						Console.WriteLine($"Результат TreeSort: {new string(symbols)}");
						return;
					default:
						continue;
				}
			}
		}

		static void QuickSort(char[] arr, int start, int end)
		{
			if (start < end)
			{
				var pivot = Partition(arr, start, end);

				QuickSort(arr, start, pivot - 1);
				QuickSort(arr, pivot + 1, end);
			}
		}

		static int Partition(char[] arr, int start, int end)
		{
			var random = new Random();
			var pivot = random.Next(start, end);

			Swap(arr, pivot, end); // move to the highest

			int i = start - 1;

			for (int j = start; j < end; j++)
			{
				if (arr[j] < arr[pivot])
				{
					i++;
					Swap(arr, i, j);
				}
			}

			Swap(arr, end, i + 1);

			return i + 1;
		}

		static void Swap(char[] arr, int i, int j)
		{
			(arr[i], arr[j]) = (arr[j], arr[i]);
		}

		static void TreeSort(char[] arr)
		{
			var tree = new Tree();
			tree.TreeInsert(arr);
			var index = 0;
			Tree.OrderTree(tree.root, arr, ref index);
		}

		static async Task RemoveRandomSymbol(string result)
		{
			if (result.Length > 0)
			{
				int value;
				var response = await client.GetAsync($"http://www.randomnumberapi.com/api/v1.0/random?min=0&max={result.Length}&count=1");
				if (response.IsSuccessStatusCode)
				{
					value = int.Parse((await response.Content.ReadAsStringAsync()).Trim().Trim(new char[] { '[', ']' }));
				}
				else
				{
					var random = new Random();
					value = random.Next(0, result.Length);
				}
				Console.WriteLine($"Обработанная строка с удаленным {value + 1} символом: {result.Remove(value, 1)}");
			}
			else
			{
				Console.WriteLine("Невозможно удалить символ из пустой строки");
			}
		}
	}

	internal class Node
	{
		public char value;

		public int count;

		public Node? left, right;

		public Node(char value)
		{
			this.value = value;
			left = null;
			right = null;
			count = 1;
		}
	}

	internal class Tree
	{
		public Node? root;

		public Tree()
		{
			root = null;
		}

		Node InsertValue(Node? root, char key)
		{
			if (root == null)
			{
				root = new Node(key);
				return root;
			}

			if (key < root.value)
				root.left = InsertValue(root.left, key);
			else if (key > root.value)
				root.right = InsertValue(root.right, key);
			else
			{
				root.count++;
			}

			return root;
		}

		public static void OrderTree(Node? root, char[] arr, ref int index)
		{
			if (root != null)
			{
				OrderTree(root.left, arr, ref index);

				for (var i = 0; i < root.count; i++)
				{
					arr[index++] = root.value;
				}

				OrderTree(root.right, arr, ref index);
			}
		}

		public void TreeInsert(char[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				root = InsertValue(root, arr[i]);
			}
		}
	}
}

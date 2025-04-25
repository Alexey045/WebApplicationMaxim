using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace WebApplicationMaxim.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MaximController : ControllerBase
	{
		private static readonly HashSet<char> vowels = new(new char[] { 'a', 'e', 'i', 'o', 'u', 'y' });
		private readonly IHttpClientFactory httpClientFactory;
		private readonly AppConfig config;

		public MaximController(IHttpClientFactory httpClientFactory, IOptions<AppConfig> config)
		{
			this.httpClientFactory = httpClientFactory;
			this.config = config.Value;
		}

		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<MaximResult>), 200)]
		public async Task<ActionResult<MaximResult>> Get(string? text, int sort)
		{
			if (text is null)
			{
				return BadRequest(new { status = 400, title = "Входная строка отсутствует" });
			}

			var badSymbols = IsLowerAscii(text);
			if (badSymbols.Count > 0)
			{
				return BadRequest(new
				{
					status = 400,
					title = $"Были введены неподходящие символы: {string.Join("", badSymbols)}"
				});
			}
			if (config.Settings!.BlackList!.Contains(text))
			{
				return BadRequest(new
				{
					status = 400,
					title = $"Строка находится в черном списке"
				});
			}

			var result = Reverse(text);

			return new MaximResult
			{

				Result1 = result,
				Result2 = WriteDictionary(CountSymbols(result)),
				Result3 = LongestVowelSubstring(result),
				Result4 = ChooseSortingAlgorithm(result, sort),
				Result5 = await RemoveRandomSymbol(result),
			};
		}

		private async Task<string> RemoveRandomSymbol(string? input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				int index;
				var client = httpClientFactory.CreateClient();
				var url = $"{config.RandomApi}?min=0&max={input.Length}&count=1";

				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					index = int.Parse((await response.Content.ReadAsStringAsync()).Trim().Trim('[', ']'));
				}
				else
				{
					index = new Random().Next(0, input.Length);
				}

				var modified = input.Remove(index, 1);

				return $"Обработанная строка с удаленным {index + 1} символом: {modified}";

			}
			else
			{
				return "Невозможно удалить символ из пустой строки";
			}
		}

		private string Reverse(string input)
		{
			var sb = new StringBuilder(input.Length);

			for (int i = input.Length - 1; i > -1; i--)
			{
				sb.Append(input[i]);
			}

			return sb.ToString();
		}

		private List<char> IsLowerAscii(string input)
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

		private Dictionary<char, int> CountSymbols(string input)
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
			}

			return symbolCount;
		}

		private string WriteDictionary(Dictionary<char, int> dict)
		{
			var sb = new StringBuilder();
			sb.Append("Количество входящих символов и их количество: ");

			foreach (var item in dict)
			{
				sb.Append($"{item.Key}-{item.Value} ");
			}

			return sb.ToString().Trim();
		}

		private string LongestVowelSubstring(string input)
		{
			var longestVowelsStringStart = -1;
			var longestVowelsStringEnd = -1;

			for (var i = 0; i < input.Length; i++)
			{
				var symbol = input[i];

				if (vowels.Contains(symbol))
				{
					if (longestVowelsStringStart == -1)
					{
						longestVowelsStringStart = i;
					}
					longestVowelsStringEnd = i;
				}
			}

			return $"Самая длинная подстрока начинающаяся и заканчивающаяся на гласную: {((longestVowelsStringStart != -1) ? input.Substring(longestVowelsStringStart, longestVowelsStringEnd - longestVowelsStringStart + 1) : "ОТСУТСТВУЕТ")}";
		}

		private string ChooseSortingAlgorithm(string input, int sort)
		{
			var symbols = input.ToCharArray();

			switch (sort)
			{
				case 1:
					QuickSort(symbols, 0, input.Length - 1);

					return $"Результат QuickSort: {new string(symbols)}";
				case 2:
					TreeSort(symbols);

					return $"Результат TreeSort: {new string(symbols)}";
			}

			return "Введен неверный код сортировки";
		}

		private void QuickSort(char[] arr, int start, int end)
		{
			if (start < end)
			{
				var pivot = Partition(arr, start, end);

				QuickSort(arr, start, pivot - 1);
				QuickSort(arr, pivot + 1, end);
			}
		}

		private int Partition(char[] arr, int start, int end)
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
	}
}

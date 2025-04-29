using System.Collections;

namespace TestProjectMaxim
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		static IEnumerable TestCases3
		{
			get
			{
				yield return new TestCaseData(
					"",
					new Dictionary<char, int> { }
				);

				yield return new TestCaseData(
					"aeea",
					new Dictionary<char, int> { { 'a', 2 }, { 'e', 2 } }
				);

				yield return new TestCaseData(
					"cbad",
					new Dictionary<char, int> { { 'c', 1 }, { 'b', 1 }, { 'a', 1 }, { 'd', 1 } }
				);
			}
		}


		[TestCase("a", "aa")]
		[TestCase("abcdeF", "cbaFed")]
		[TestCase("abcde", "edcbaabcde")]
		[TestCase("", "")]
		[TestCase("123!456", "654!321123!456")]
		public void Test1(string input, string expected)
		{
			Assert.That(MaximController.ReverseText(input), Is.EqualTo(expected));
		}

		[TestCase("", new char[] { })]
		[TestCase("hello", new char[] { })]
		[TestCase("abc1", new char[] { '1' })]
		[TestCase("аbс", new char[] { 'а', 'с' })] // кириллица
		[TestCase("Hello world!", new char[] { 'H', ' ', '!' })]
		public void Test2(string input, char[] expected)
		{
			Assert.That(MaximController.IsLowerAscii(input).ToArray(), Is.EqualTo(expected));
		}

		[TestCaseSource(nameof(TestCases3))]
		public void Test3(string input, Dictionary<char, int> expected)
		{
			Assert.That(MaximController.CountSymbols(input), Is.EqualTo(expected));
		}

		[TestCase("edcbaabcde", "edcbaabcde")]
		[TestCase("cc", "")]
		[TestCase("lollol", "ollo")]
		public void Test4(string input, string expected)
		{
			Assert.That(MaximController.LongestVowelSubstring(input), Is.EqualTo(expected));
		}

		[TestCase("", 1, "–езультат QuickSort: ")]
		[TestCase("edcbaabcde", 1, "–езультат QuickSort: aabbccddee")]
		[TestCase("edcbaabcde", 2, "–езультат TreeSort: aabbccddee")]
		[TestCase("", 2, "–езультат TreeSort: ")]
		[TestCase("edcbaabcde", 10, "¬веден неверный код сортировки")]
		[TestCase("", -4, "¬веден неверный код сортировки")]
		[TestCase("edcbaabcde", 0, "¬веден неверный код сортировки")]
		public void Test5(string input, int sort, string expected)
		{
			Assert.That(MaximController.ChooseSortingAlgorithm(input, sort), Is.EqualTo(expected));
		}
	}
}
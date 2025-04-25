namespace WebApplicationMaxim
{
	public class Node
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
}

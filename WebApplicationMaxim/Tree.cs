namespace WebApplicationMaxim
{
	public class Tree
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

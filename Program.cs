using System;
using B_trees_Implementation;

BTree tree = new BTree();

int[] values = { 10, 20, 5, 6, 12, 30, 7, 17, 3, 4, 15 };

//insert all values
foreach (int value in values)
{
    tree.insert(value);
}

Console.WriteLine("b tree:");
tree.display();

Console.WriteLine();

//search
Console.WriteLine("search 15: " + tree.search(15));
Console.WriteLine("search 100: " + tree.search(100));

Console.WriteLine();

//delete
tree.delete(15);

Console.WriteLine("after deleting 15:");
tree.display();
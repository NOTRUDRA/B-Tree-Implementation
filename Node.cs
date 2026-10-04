namespace B_trees_Implementation;

public class BTreeNode
{
    //node can store up to 3 keys
    public int[] keys = new int[3];

    //node can have up to 4 children
    public BTreeNode[] kids = new BTreeNode[4];

    //how many keys has been filled
    public int count;

    //is the node leaf or not
    public bool leaf;

    //construct
    public BTreeNode(bool isleaf)
    {
        leaf = isleaf;
        count = 0;
    }
}
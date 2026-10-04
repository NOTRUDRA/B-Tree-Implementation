using System;

namespace B_trees_Implementation;

public class BTree
{
    //top node of the tree
    private BTreeNode root;

    //add a key to the tree
    public void insert(int key)
    {
        //do nothing if key is already there
        if (search(key))
            return;

        //tree is empty so make the first leaf
        if (root == null)
        {
            root = new BTreeNode(true);
            root.keys[0] = key;
            root.count = 1;
            return;
        }

        //root is full so make a new root above it and split
        if (root.count == 3)
        {
            BTreeNode newroot = new BTreeNode(false);
            newroot.kids[0] = root;
            root = newroot;
            splitkid(root, 0);
        }

        insertnonfull(root, key);
    }

    //insert key into a node that is not full
    private void insertnonfull(BTreeNode node, int key)
    {
        int i = node.count - 1;

        if (node.leaf)
        {
            //move bigger keys one step right
            while (i >= 0 && key < node.keys[i])
            {
                node.keys[i + 1] = node.keys[i];
                i--;
            }

            //put the key in the gap
            node.keys[i + 1] = key;
            node.count++;
        }
        else
        {
            //find which child to go to
            while (i >= 0 && key < node.keys[i])
            {
                i--;
            }
            i++;

            //split the child first if it is full
            if (node.kids[i].count == 3)
            {
                splitkid(node, i);

                //middle key came up so check which side to go
                if (key > node.keys[i])
                    i++;
            }

            insertnonfull(node.kids[i], key);
        }
    }

    //split a full child into two nodes
    private void splitkid(BTreeNode parent, int index)
    {
        //the full child with 3 keys
        BTreeNode full = parent.kids[index];

        //new node takes the last key of the full child
        BTreeNode newkid = new BTreeNode(full.leaf);
        newkid.keys[0] = full.keys[2];
        newkid.count = 1;

        //if not leaf, new node also takes the last 2 children
        if (!full.leaf)
        {
            newkid.kids[0] = full.kids[2];
            newkid.kids[1] = full.kids[3];
        }

        //make space in parent for the new child
        for (int i = parent.count; i > index; i--)
        {
            parent.kids[i + 1] = parent.kids[i];
        }
        parent.kids[index + 1] = newkid;

        //make space in parent for the middle key
        for (int i = parent.count - 1; i >= index; i--)
        {
            parent.keys[i + 1] = parent.keys[i];
        }

        //middle key goes up to parent
        parent.keys[index] = full.keys[1];
        parent.count++;

        //full child keeps only its first key
        full.count = 1;
    }

    //check if key is in the tree
    public bool search(int key)
    {
        if (root == null)
            return false;

        return searchnode(root, key);
    }

    private bool searchnode(BTreeNode node, int key)
    {
        //find first key that is not smaller
        int i = 0;
        while (i < node.count && key > node.keys[i])
        {
            i++;
        }

        //found it
        if (i < node.count && key == node.keys[i])
            return true;

        //leaf has nowhere else to go
        if (node.leaf)
            return false;

        //go down to the child
        return searchnode(node.kids[i], key);
    }

    //remove a key from the tree
    public void delete(int key)
    {
        if (root == null)
            return;

        deletenode(root, key);

        //if root has no keys left then tree gets shorter
        if (root.count == 0)
        {
            if (root.leaf)
                root = null;
            else
                root = root.kids[0];
        }
    }

    private void deletenode(BTreeNode node, int key)
    {
        int index = findkey(node, key);

        //key is in this node
        if (index < node.count && node.keys[index] == key)
        {
            if (node.leaf)
                removeleaf(node, index);
            else
                removeinternal(node, index);
        }
        else
        {
            //key is not in the tree
            if (node.leaf)
                return;

            //child has only 1 key so give it more before going down
            if (node.kids[index].count < 2)
            {
                if (index > 0 && node.kids[index - 1].count >= 2)
                {
                    borrowprev(node, index);
                }
                else if (index < node.count && node.kids[index + 1].count >= 2)
                {
                    borrownext(node, index);
                }
                else
                {
                    //no one can lend so merge with a neighbour
                    if (index < node.count)
                    {
                        merge(node, index);
                    }
                    else
                    {
                        merge(node, index - 1);
                        index--;
                    }
                }
            }

            deletenode(node.kids[index], key);
        }
    }

    //find the first position where key could be
    private int findkey(BTreeNode node, int key)
    {
        int i = 0;
        while (i < node.count && node.keys[i] < key)
        {
            i++;
        }
        return i;
    }

    //remove a key from a leaf by moving keys left
    private void removeleaf(BTreeNode node, int index)
    {
        for (int i = index + 1; i < node.count; i++)
        {
            node.keys[i - 1] = node.keys[i];
        }
        node.count--;
    }

    //remove a key from a node that has children
    private void removeinternal(BTreeNode node, int index)
    {
        int key = node.keys[index];

        if (node.kids[index].count >= 2)
        {
            //replace with biggest key on the left side
            int pre = predecessor(node, index);
            node.keys[index] = pre;
            deletenode(node.kids[index], pre);
        }
        else if (node.kids[index + 1].count >= 2)
        {
            //replace with smallest key on the right side
            int suc = successor(node, index);
            node.keys[index] = suc;
            deletenode(node.kids[index + 1], suc);
        }
        else
        {
            //both sides are small so merge them then delete
            merge(node, index);
            deletenode(node.kids[index], key);
        }
    }

    //biggest key on the left side
    private int predecessor(BTreeNode node, int index)
    {
        BTreeNode cur = node.kids[index];
        while (!cur.leaf)
        {
            cur = cur.kids[cur.count];
        }
        return cur.keys[cur.count - 1];
    }

    //smallest key on the right side
    private int successor(BTreeNode node, int index)
    {
        BTreeNode cur = node.kids[index + 1];
        while (!cur.leaf)
        {
            cur = cur.kids[0];
        }
        return cur.keys[0];
    }

    //take a key from the left neighbour through the parent
    private void borrowprev(BTreeNode node, int index)
    {
        BTreeNode kid = node.kids[index];
        BTreeNode sib = node.kids[index - 1];

        //make space at the front of the child
        for (int i = kid.count - 1; i >= 0; i--)
        {
            kid.keys[i + 1] = kid.keys[i];
        }

        if (!kid.leaf)
        {
            for (int i = kid.count; i >= 0; i--)
            {
                kid.kids[i + 1] = kid.kids[i];
            }
            kid.kids[0] = sib.kids[sib.count];
        }

        //parent key comes down, neighbour key goes up
        kid.keys[0] = node.keys[index - 1];
        node.keys[index - 1] = sib.keys[sib.count - 1];

        kid.count++;
        sib.count--;
    }

    //take a key from the right neighbour through the parent
    private void borrownext(BTreeNode node, int index)
    {
        BTreeNode kid = node.kids[index];
        BTreeNode sib = node.kids[index + 1];

        //parent key comes down to the end of the child
        kid.keys[kid.count] = node.keys[index];

        if (!kid.leaf)
        {
            kid.kids[kid.count + 1] = sib.kids[0];
        }

        //neighbour key goes up to the parent
        node.keys[index] = sib.keys[0];

        //close the gap in the neighbour
        for (int i = 1; i < sib.count; i++)
        {
            sib.keys[i - 1] = sib.keys[i];
        }

        if (!sib.leaf)
        {
            for (int i = 1; i <= sib.count; i++)
            {
                sib.kids[i - 1] = sib.kids[i];
            }
        }

        kid.count++;
        sib.count--;
    }

    //join child and its right neighbour with the parent key between them
    private void merge(BTreeNode node, int index)
    {
        BTreeNode kid = node.kids[index];
        BTreeNode sib = node.kids[index + 1];

        //parent key comes down into the child
        kid.keys[1] = node.keys[index];

        //copy all keys of neighbour
        for (int i = 0; i < sib.count; i++)
        {
            kid.keys[i + 2] = sib.keys[i];
        }

        //copy all children of neighbour
        if (!kid.leaf)
        {
            for (int i = 0; i <= sib.count; i++)
            {
                kid.kids[i + 2] = sib.kids[i];
            }
        }

        kid.count = kid.count + sib.count + 1;

        //close the gap in the parent keys
        for (int i = index + 1; i < node.count; i++)
        {
            node.keys[i - 1] = node.keys[i];
        }

        //close the gap in the parent children
        for (int i = index + 2; i <= node.count; i++)
        {
            node.kids[i - 1] = node.kids[i];
        }

        node.count--;
    }

    //print the whole tree
    public void display()
    {
        if (root == null)
        {
            Console.WriteLine("tree is empty");
            return;
        }

        displaynode(root, 0);
    }

    private void displaynode(BTreeNode node, int level)
    {
        Console.Write("level " + level + ": ");

        for (int i = 0; i < node.count; i++)
        {
            Console.Write(node.keys[i] + " ");
        }

        Console.WriteLine();

        if (!node.leaf)
        {
            for (int i = 0; i <= node.count; i++)
            {
                displaynode(node.kids[i], level + 1);
            }
        }
    }
}
public class Node
{
    public bool walkable = true;
    public int x,y;
    public Slot slot;

    public int gCost;//distance from start node
    public int hCost;//distance from end node

    public int fCost => gCost + hCost;
    public Node parent;

    public Node(int x,int y, Slot slot)
    {
        if (slot == null)
            walkable = false;
        else
        {
            this.slot = slot;
            if (slot.Mob != null || slot.Tunnel != null)
                walkable = false;
        }
        this.x = x;
        this.y = y;
    }


}

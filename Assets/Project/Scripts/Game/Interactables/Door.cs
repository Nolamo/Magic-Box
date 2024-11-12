using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDoor
{
    public bool isLocked { get; set; }
    public bool isOpen { get; set; }

    public virtual void Open()
    {
        if (isLocked) return;
        isOpen = true;
    }

    public virtual void Close() 
    {
        isOpen = false;
    }

    public virtual void Lock()
    {
        isLocked = true;
    }

    public virtual void Unlock()
    {
        isLocked = false;
    }
}

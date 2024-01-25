using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerLocalDataHandler
{
    public bool BDirty { get; set; }
    public bool BNowSave { get; set; }
}

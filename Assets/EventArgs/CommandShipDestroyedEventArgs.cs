using System;

public class CommandShipDestroyedEventArgs : EventArgs
{
    private CommandShip _commandShip = null;


    public CommandShipDestroyedEventArgs(CommandShip commandShip)
    {
        _commandShip = commandShip;
    }


    public CommandShip CommandShip
    {
        get { return _commandShip; }
    }
}
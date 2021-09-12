using System;
namespace NetConnMon.Domain.Enums
{
    public enum NetProtocol
    {
        ICMP, // as a Ping
        UDP,  // as a GET request
        TCP
    };

    public enum DataChangeType
    {
        NoChange,
        Add,
        Edit,
        Delete
    };
}

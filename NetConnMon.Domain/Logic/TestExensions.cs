using System;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;

namespace NetConnMon.Domain.Logic
{
    public static class TestExensions
    {
        public static TestDefinition SetPortDefaults(this TestDefinition testDefinition)
        {
            if (testDefinition.Port == 0)
                testDefinition.SetPortByProtocol();

            return testDefinition;
        }
        public static TestDefinition SetPortByProtocol(this TestDefinition testDefinition)
        {
            testDefinition.Port =
                testDefinition.Protocol switch
                {
                    NetProtocol.TCP => 80,
                    NetProtocol.ICMP => 0,
                    NetProtocol.UDP => 7,
                    _ => 0
                };

            return testDefinition;
        }
    }
}

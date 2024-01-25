using System;

namespace SNShien.Common.NetworkTools
{
    public interface IServerCommunicator
    {
        event Action OnRequestCompleted;
        bool IsWaitingResponse { get; }
        ServerCommunicator CreatePostRequest(string url, string action = null);
    }
}
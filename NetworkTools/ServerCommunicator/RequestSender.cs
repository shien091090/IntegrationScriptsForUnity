using System;

namespace SNShien.Common.NetworkTools
{
    public class RequestSender
    {
        public string Url { get; }
        public string Action { get; }
        public RequestType RequestType { get; }
        public (string, string)[] Parameters { get; private set; }

        public RequestSender(string url, RequestType requestType, string action = null)
        {
            Url = url;
            RequestType = requestType;
            Action = string.IsNullOrEmpty(action) ?
                string.Empty :
                action;
        }

        public void AddParameter(string fieldName, string fieldValue)
        {
            if (Parameters == null)
                Parameters = Array.Empty<(string, string)>();

            (string, string)[] newParameters = new (string, string)[Parameters.Length + 1];
            for (int i = 0; i < Parameters.Length; i++)
            {
                newParameters[i] = Parameters[i];
            }

            newParameters[Parameters.Length] = (fieldName, fieldValue);
            Parameters = newParameters;
        }
    }
}
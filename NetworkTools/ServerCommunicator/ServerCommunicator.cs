using System;
using System.Collections;
using SNShien.Common.DataTools;
using UnityEngine;
using UnityEngine.Networking;

namespace SNShien.Common.NetworkTools
{
    public class ServerCommunicator : MonoBehaviour, IServerCommunicator
    {
        public bool IsWaitingResponse { get; private set; }

        private RequestSender requestSender;
        public event Action OnRequestCompleted;

        public ServerCommunicator CreatePostRequest(string url, string action = null)
        {
            requestSender = new RequestSender(url, RequestType.Post, action);
            return this;
        }

        public ServerCommunicator AddParameter(string filedName, object fieldValue)
        {
            if (requestSender == null)
                return this;

            requestSender.AddParameter(filedName, fieldValue.ToString());
            return this;
        }

        public void SendRequest<T>(Action<T> callback = null) where T : ServerResponse
        {
            if (requestSender == null)
            {
                Debug.Log("[ServerCommunicator] requestSender is null");
                return;
            }

            StartCoroutine(Cor_SendRequest(requestSender, callback));
        }

        private WWWForm CreateWWWForm(string action, (string, string)[] parameters)
        {
            WWWForm form = new WWWForm();
            form.AddField("action", action);
            if (parameters != null)
            {
                foreach ((string fieldName, string fieldValue) parameter in parameters)
                {
                    if (int.TryParse(parameter.fieldValue, out int parseIntValue))
                        form.AddField(parameter.fieldName, parseIntValue);
                    else
                        form.AddField(parameter.fieldName, parameter.fieldValue);
                }
            }

            return form;
        }

        private T CreateErrorResponse<T>(UnityWebRequest requestInfo) where T : ServerResponse
        {
            T errorResponse = Activator.CreateInstance<T>();
            errorResponse.SetError(requestInfo.error);
            return errorResponse;
        }

        private IEnumerator Cor_SendRequest<T>(RequestSender requestInfo, Action<T> callback) where T : ServerResponse
        {
            Debug.Log($"[ServerCommunicator] SendRequest, action: {requestInfo.Action}, requestType: {requestInfo.RequestType}");
            WWWForm form = CreateWWWForm(requestInfo.Action, requestInfo.Parameters);

            IsWaitingResponse = true;
            if (requestInfo.RequestType == RequestType.Post)
            {
                using (UnityWebRequest www = UnityWebRequest.Post(requestInfo.Url, form))
                {
                    yield return www.SendWebRequest();

                    IsWaitingResponse = false;

                    if (www.result != UnityWebRequest.Result.Success || string.IsNullOrEmpty(www.downloadHandler.text))
                        callback?.Invoke(CreateErrorResponse<T>(www));
                    else
                    {
                        Debug.Log($"[ServerCommunicator] Response, action: {requestInfo.Action}, content: {www.downloadHandler.text}");
                        JsonParser jsonParser = new JsonParser();
                        callback?.Invoke(jsonParser.TryDeserializeObject(www.downloadHandler.text, out T response) ?
                            response :
                            CreateErrorResponse<T>(www));
                    }

                    OnRequestCompleted?.Invoke();
                }
            }
        }
    }
}
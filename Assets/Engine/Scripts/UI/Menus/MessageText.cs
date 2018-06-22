using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class MessageText : MonoBehaviour
    {
        public UILabel Msg;
        public Queue<string> msgQueue = new Queue<string>();

        private void Start()
        {
            StartCoroutine(CheckMsgRoutine());
        }

        public void Show(string msg)
        {
            msgQueue.Enqueue(msg);
        }

        public void Clear()
        {
            msgQueue.Clear();
        }

        private IEnumerator CheckMsgRoutine()
        {
            while (true)
            {
                if (msgQueue.Count > 0)
                {
                    string msgText = msgQueue.Dequeue();
                    Msg.transform.localScale = Vector3.zero;
                    Msg.text = msgText;
                    Msg.gameObject.ScaleTo(Vector3.one, 1f, 0f, EaseType.spring);
                    yield return new WaitForSeconds(1.5f);
                    Msg.gameObject.ScaleTo(Vector3.zero, 0.5f, 0f);
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
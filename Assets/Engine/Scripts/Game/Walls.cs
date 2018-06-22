using UnityEngine;

namespace MB_Engine
{
    public class Walls : MonoBehaviour
    {
        #region fields
        public GameObject LeftWall3D;
        public GameObject RightWall3D;
        public GameObject TopWall3D;
        public GameObject BottomWall3D;
        #endregion

        private void OnEnable()
        {
            CorrectWalls();
        }

        private void CorrectWalls()
        {
            Camera cam = GameManager.main.CamController.Cam;
            Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z + 0.25f));

            // Left wall
            Vector3 wallPos = LeftWall3D.transform.position;
            wallPos.x = -worldPoint.x;
            LeftWall3D.transform.position = wallPos;

            // Right wall
            wallPos = RightWall3D.transform.position;
            wallPos.x = worldPoint.x;
            RightWall3D.transform.position = wallPos;

            // Top wall
            //wallPos = TopWall3D.transform.position;
            //wallPos.y = worldPoint.y;
            //TopWall3D.transform.position = wallPos;

            // Bottom wall
            //wallPos = BottomWall3D.transform.position;
            //wallPos.y = -worldPoint.y - 1f;
            //BottomWall3D.transform.position = wallPos;

            //Back wall
            //float x = worldPoint.x / 2.51f;
            //float y = worldPoint.y / 2.51f;
        }
    }
}
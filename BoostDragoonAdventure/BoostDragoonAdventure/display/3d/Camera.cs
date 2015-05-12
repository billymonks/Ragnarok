using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;
using wickedcrush.manager.controls;
using wickedcrush.manager.player;

namespace wickedcrush.display._3d
{
    public enum CameraMode
    {
        Still = 0,
        Follow = 1
    }
    public class Camera
    {
        #region fields
        public Vector3 cameraPosition, cameraTarget, upVector, velocity;
        public Vector2 minCamPos, maxCamPos;
        //public Vector2
        public float fov, minX, minY;
        public Entity target;
        public CameraMode camMode = CameraMode.Still;
        public float smoothVal = 10f;

        public ControlsManager _controls;
        public PlayerManager _players;
        public float zoom = 2f; //1f = zoomed in, 2f = zoomed out
        #endregion

        public Camera(PlayerManager players)
        {
            cameraPosition = new Vector3(0f, 0f, 0f);
            cameraTarget = new Vector3(0f, 0f, 0f);
            velocity = new Vector3(0f, 0f, 0f);
            upVector = Vector3.Up;
            fov = MathHelper.PiOver4;
            minCamPos = new Vector2(120f, 64f);
            maxCamPos = new Vector2(524f, 320f);

            _players = players;
        }

        public void SetTarget(Entity e)
        {
            target = e;
            camMode = CameraMode.Follow;

            cameraPosition.X = target.pos.X;
            cameraPosition.Y = target.pos.Y;
            cameraTarget.X = target.pos.X;
            cameraTarget.Y = target.pos.Y;
        }

        public void setCameraDepth(float nDepth)
        {
            cameraPosition.Z = nDepth;
        }

        public void Update() // to be updated with use of state machine...maybe
        {
            /*if (target == null)
            {
                camMode = CameraMode.Still;
            }

            switch (camMode)
            {
                case CameraMode.Still:
                    break;
                case CameraMode.Follow:
                    cameraPosition.X = (cameraPosition.X * smoothVal + target.pos.X) / (smoothVal + 1f);
                    cameraTarget.X = cameraPosition.X;
                    cameraPosition.Y = (cameraPosition.Y * smoothVal + target.pos.Y) / (smoothVal + 1f);
                    cameraTarget.Y = cameraPosition.Y;
                    break;
            }

            cameraPosition += velocity;
            cameraTarget += velocity;*/

            float looseness = 10f;

            cameraPosition.X = ((cameraPosition.X * looseness) + (_players.getMeanPlayerPos().X - 320)) / (looseness+1f); //todo: change everything
            cameraPosition.Y = ((cameraPosition.Y * looseness) + (_players.getMeanPlayerPos().Y - 240)) / (looseness + 1f);

            //adhereToBounds();
        }
        private void adhereToBounds()
        {
            if (cameraPosition.X + minCamPos.X > maxCamPos.X)
            {
                cameraPosition.X = maxCamPos.X - minCamPos.X;
                cameraTarget.X = maxCamPos.X - minCamPos.X;
            }
            if (cameraPosition.Y + minCamPos.Y > maxCamPos.Y)
            {
                cameraPosition.Y = maxCamPos.Y - minCamPos.Y;
                cameraTarget.Y = maxCamPos.Y - minCamPos.Y;
            }
            if (cameraPosition.X < minCamPos.X)
            {
                cameraPosition.X = minCamPos.X;
                cameraTarget.X = minCamPos.X;
            }
            if (cameraPosition.Y < minCamPos.Y)
            {
                cameraPosition.Y = minCamPos.Y;
                cameraTarget.Y = minCamPos.Y;
            }
        }
        public void MoveCamLeft(float speed)
        {
            cameraPosition.X = cameraPosition.X - speed;
            cameraTarget.X = cameraTarget.X - speed;
            adhereToBounds();
        }
        public void MoveCamRight(float speed)
        {
            cameraPosition.X = cameraPosition.X + speed;
            cameraTarget.X = cameraTarget.X + speed;
        }
        public void MoveCamUp(float speed)
        {
            cameraPosition.Y = cameraPosition.Y + speed;
            cameraTarget.Y = cameraTarget.Y + speed;
        }
        public void MoveCamDown(float speed)
        {
            cameraPosition.Y = cameraPosition.Y - speed;
            cameraTarget.Y = cameraTarget.Y - speed;
            adhereToBounds();
        }
    }
}

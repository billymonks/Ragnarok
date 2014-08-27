using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;
using wickedcrush.manager.controls;

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
        public Vector2 minCamPos;
        public float fov, minX, minY;
        public Entity target;
        public CameraMode camMode = CameraMode.Still;
        public float smoothVal = 7f;

        public ControlsManager _controls;
        #endregion

        public Camera()
        {
            cameraPosition = new Vector3(0f, 0f, 0f);
            cameraTarget = new Vector3(0f, 0f, 0f);
            velocity = new Vector3(0f, 0f, 0f);
            upVector = Vector3.Up;
            fov = MathHelper.PiOver4;
            minCamPos = new Vector2(356f, 448f);
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
            if (target == null)
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
            cameraTarget += velocity;
            //adhereToBounds();
        }
        private void adhereToBounds()
        {
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

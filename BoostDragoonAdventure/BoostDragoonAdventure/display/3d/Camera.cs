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
        public Random random = new Random();
        public float screenShakeAmount = 0f;

        float looseness = 50f;
        public float targetLooseness = 50f;
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

        public void ShakeScreen(float amount)
        {
            screenShakeAmount = Math.Max(amount, screenShakeAmount);
            //screenShakeAmount += amount;
        }

        public void setCameraDepth(float nDepth)
        {
            cameraPosition.Z = nDepth;
        }

        public void Update(GameTime gameTime) // to be updated with use of state machine...maybe
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
            

            float stretch = 0.03f;

            looseness = (looseness + targetLooseness * stretch) / (stretch+1f);

            cameraPosition.X = (float)(screenShakeAmount * ((Generate((float)gameTime.TotalGameTime.Milliseconds / 50f)))) + (((cameraPosition.X * looseness) + (_players.getMeanPlayerPos().X - 320)) / (looseness+1f)); //todo: change everything
            cameraPosition.Y = (float)(screenShakeAmount * ((Generate((float)gameTime.TotalGameTime.Milliseconds / 100f)))) + (((cameraPosition.Y * looseness) + (_players.getMeanPlayerPos().Y - 240)) / (looseness + 1f));

            screenShakeAmount /= 2f;
            //adhereToBounds();
        }

        public static float Generate(float x)
        {
            int i0 = FastFloor(x);
            int i1 = i0 + 1;
            float x0 = x - i0;
            float x1 = x0 - 1.0f;

            float n0, n1;

            float t0 = 1.0f - x0 * x0;
            t0 *= t0;
            n0 = t0 * t0 * grad(perm[i0 & 0xff], x0);

            float t1 = 1.0f - x1 * x1;
            t1 *= t1;
            n1 = t1 * t1 * grad(perm[i1 & 0xff], x1);
            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 scales to fit exactly within [-1,1]
            return 0.395f * (n0 + n1);
        }

        public static byte[] perm = new byte[512] { 151,160,137,91,90,15,
              131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
              190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
              88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
              77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
              102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
              135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
              5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
              223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
              129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
              251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
              49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
              138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
              151,160,137,91,90,15,
              131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
              190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
              88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
              77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
              102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
              135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
              5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
              223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
              129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
              251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
              49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
              138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180 
            };

        private static int FastFloor(float x)
        {
            return (x > 0) ? ((int)x) : (((int)x) - 1);
        }

        private static float grad(int hash, float x)
        {
            int h = hash & 15;
            float grad = 1.0f + (h & 7);   // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) != 0) grad = -grad;         // Set a random sign for the gradient
            return (grad * x);           // Multiply the gradient with the distance
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

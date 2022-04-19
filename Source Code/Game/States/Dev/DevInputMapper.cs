using Engine2D.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Game.Input
{
    public class DevInputMapper : BaseInputMapper
    {
        public override IEnumerable<BaseInputCommand> GetKeyboardState(KeyboardState state)
        {
            var commands = new List<DevInputCommand>();

            if (state.IsKeyDown(Keys.Escape))
            {
                commands.Add(new DevInputCommand.DevQuit());
            }

            if (state.IsKeyDown(Keys.Z))
            {
                commands.Add(new DevInputCommand.DevBulletSparks());
            }

            if (state.IsKeyDown(Keys.X))
            {
                commands.Add(new DevInputCommand.DevMissileExplode());
            }

            if (state.IsKeyDown(Keys.C))
            {
                commands.Add(new DevInputCommand.DevExplode());
            }

            if (state.IsKeyDown(Keys.Right))
            {
                commands.Add(new DevInputCommand.DevCamRight());
            }

            if (state.IsKeyDown(Keys.Left))
            {
                commands.Add(new DevInputCommand.DevCamLeft());
            }

            if (state.IsKeyDown(Keys.Up))
            {
                commands.Add(new DevInputCommand.DevCamUp());
            }

            if (state.IsKeyDown(Keys.Down))
            {
                commands.Add(new DevInputCommand.DevCamDown());
            }

            if (state.IsKeyDown(Keys.Space))
            {
                commands.Add(new DevInputCommand.DevShoot());
            }

            if (state.IsKeyDown(Keys.O))
            {
                commands.Add(new DevInputCommand.DevCamRotateLeft());
            }

            if (state.IsKeyDown(Keys.W))
            {
                commands.Add(new DevInputCommand.DevPlayerUp());
            }
            else if (state.IsKeyDown(Keys.S))
            {
                commands.Add(new DevInputCommand.DevPlayerDown());
            }
            else
            {
                commands.Add(new DevInputCommand.DevPlayerStopsMovingVertical());
            }

            if (state.IsKeyDown(Keys.A))
            {
                commands.Add(new DevInputCommand.DevPlayerLeft());
            } 
            else if (state.IsKeyDown(Keys.D))
            {
                commands.Add(new DevInputCommand.DevPlayerRight());
            }
            else
            {
                commands.Add(new DevInputCommand.DevPlayerStopsMovingHorizontal());
            }

            if (state.IsKeyDown(Keys.P))
            {
                commands.Add(new DevInputCommand.DevCamRotateRight());
            }


            return commands;
        }
    }
}

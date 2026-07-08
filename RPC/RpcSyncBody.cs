using Crewmeleon.Roles;
using FungleAPI.Base.Rpc;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Sentry.Unity.NativeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.RPC
{
    internal class RpcSyncBody : SimpleRpc<NetworkedPlayerInfo>
    {
        public override void Write(MessageWriter messageWriter)
        {
            IEnumerable<KeyValuePair<Vector2Int, Color32>> batch = ChameleonRole.Local.PaintStrokes.Take(100);

            messageWriter.Write((byte)batch.Count());
            foreach (KeyValuePair<Vector2Int, Color32> pair in batch)
            {
                ChameleonRole.Local.PaintStrokes.Remove(pair.Key);

                ushort index = (ushort)(pair.Key.y * 165 + pair.Key.x);
                messageWriter.Write(index);
                messageWriter.Write(pair.Value.r);
                messageWriter.Write(pair.Value.g);
                messageWriter.Write(pair.Value.b);
            }
        }
        public override void Handle(NetworkedPlayerInfo innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject.Role.Is(out ChameleonRole c))
            {
                Texture2D texture2D = c.Chameleon.sprite.texture;

                byte pixelCount = messageReader.ReadByte();

                for (int i = 0; i < pixelCount; i++)
                {
                    ushort index = messageReader.ReadUInt16();

                    int x = index % 165;
                    int y = index / 165;
                    byte r = messageReader.ReadByte();
                    byte g = messageReader.ReadByte();
                    byte b = messageReader.ReadByte();

                    c.PaintBrush(new Vector2Int(x, y), new Color32(r, g, b, byte.MaxValue));
                }

                texture2D.Apply();
            }
        }
    }
}

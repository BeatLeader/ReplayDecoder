using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReplayDecoder
{
    public class Replay
    {
        public ReplayInfo info = new ReplayInfo();

        public List<Frame> frames = new List<Frame>();

        public List<NoteEvent> notes = new List<NoteEvent>();
        public List<WallEvent> walls = new List<WallEvent>();
        public List<AutomaticHeight> heights = new List<AutomaticHeight>();
        public List<Pause> pauses = new List<Pause>();
        public SaberOffsets saberOffsets = new SaberOffsets();
        public Dictionary<string, byte[]> customData = new Dictionary<string, byte[]>();
    }

    public class ReplayInfo
    {
        public string version;
        public string gameVersion;
        public string timestamp;
        
        public string playerID;
        public string playerName;
        public string platform;

        public string trackingSytem;
        public string hmd;
        public string controller;

        public string hash;
        public string songName;
        public string mapper;
        public string difficulty;

        public int score;
        public string mode;
        public string environment;
        public string modifiers;
        public float jumpDistance;
        public bool leftHanded;
        public float height;

        public float startTime;
        public float failTime;
        public float speed;
    }

    public class Frame
    {
        public float time;
        public int fps;
        public Transform head;
        public Transform leftHand;
        public Transform rightHand;

        public void ToArray(float[,] array, int index, Frame previous)
        {
            array[index, 0] = (time - previous.time);

            array[index, 1] = head.position.x;
            array[index, 2] = head.position.y;
            array[index, 3] = head.position.z;
            array[index, 4] = head.rotation.x;
            array[index, 5] = head.rotation.y;
            array[index, 6] = head.rotation.z;
            array[index, 7] = head.rotation.w;


            array[index, 7 + 1] = leftHand.position.x;
            array[index, 7 + 2] = leftHand.position.y;
            array[index, 7 + 3] = leftHand.position.z;
            array[index, 7 + 4] = leftHand.rotation.x;
            array[index, 7 + 5] = leftHand.rotation.y;
            array[index, 7 + 6] = leftHand.rotation.z;
            array[index, 7 + 7] = leftHand.rotation.w;


            array[index, 14 + 1] = rightHand.position.x;
            array[index, 14 + 2] = rightHand.position.y;
            array[index, 14 + 3] = rightHand.position.z;
            array[index, 14 + 4] = rightHand.rotation.x;
            array[index, 14 + 5] = rightHand.rotation.y;
            array[index, 14 + 6] = rightHand.rotation.z;
            array[index, 14 + 7] = rightHand.rotation.w;
        }
        public void ToArray(float[,] array, int index)
        {
            array[index, 0] = head.position.x;
            array[index, 1] = head.position.y;
            array[index, 2] = head.position.z;
            array[index, 3] = head.rotation.x;
            array[index, 4] = head.rotation.y;
            array[index, 5] = head.rotation.z;
            array[index, 6] = head.rotation.w;


            array[index, 6 + 1] = leftHand.position.x;
            array[index, 6 + 2] = leftHand.position.y;
            array[index, 6 + 3] = leftHand.position.z;
            array[index, 6 + 4] = leftHand.rotation.x;
            array[index, 6 + 5] = leftHand.rotation.y;
            array[index, 6 + 6] = leftHand.rotation.z;
            array[index, 6 + 7] = leftHand.rotation.w;


            array[index, 13 + 1] = rightHand.position.x;
            array[index, 13 + 2] = rightHand.position.y;
            array[index, 13 + 3] = rightHand.position.z;
            array[index, 13 + 4] = rightHand.rotation.x;
            array[index, 13 + 5] = rightHand.rotation.y;
            array[index, 13 + 6] = rightHand.rotation.z;
            array[index, 13 + 7] = rightHand.rotation.w;
        }
    };

    public enum NoteEventType
    {
        good = 0,
        bad = 1,
        miss = 2,
        bomb = 3
    }

    public class NoteEvent
    {
        public int noteID;
        public float eventTime;
        public float spawnTime;
        public NoteEventType eventType;
        public NoteCutInfo noteCutInfo;

        public NoteScore score;
        public NoteParams noteParams;
    };

    public class WallEvent
    {
        public int wallID;
        public float energy;
        public float time;
        public float spawnTime;
    };

    public class AutomaticHeight
    {
        public float height;
        public float time;
    };

    public class Pause
    {
        public long duration;
        public float time;
    };

    public class SaberOffsets {
        public Vector3 leftSaberLocalPosition;
        public Quaternion leftSaberLocalRotation;
        public Vector3 rightSaberLocalPosition;
        public Quaternion rightSaberLocalRotation;
    };

    public class NoteCutInfo
    {
        public bool speedOK;
        public bool directionOK;
        public bool saberTypeOK;
        public bool wasCutTooSoon;
        public float saberSpeed;
        public Vector3 saberDir;
        public int saberType;
        public float timeDeviation;
        public float cutDirDeviation;
        public Vector3 cutPoint;
        public Vector3 cutNormal;
        public float cutDistanceToCenter;
        public float cutAngle;
        public float beforeCutRating;
        public float afterCutRating;
    };

    public enum StructType
    {
        info = 0,
        frames = 1,
        notes = 2,
        walls = 3,
        heights = 4,
        pauses = 5,
        saberOffsets = 6,
        customData = 7
    }

    public struct Vector3
    {
        public const float kEpsilonNormalSqrt = 1e-15F;
        public const float kEpsilon = 0.00001F;

        public float x;
        public float y;
        public float z;

        public Vector3(float v1, float v2, float v3) : this()
        {
            x = v1;
            y = v2;
            z = v3;
        }

        static readonly Vector3 zeroVector = new Vector3(0F, 0F, 0F);
        static readonly Vector3 oneVector = new Vector3(1F, 1F, 1F);
        static readonly Vector3 upVector = new Vector3(0F, 1F, 0F);
        static readonly Vector3 downVector = new Vector3(0F, -1F, 0F);
        static readonly Vector3 leftVector = new Vector3(-1F, 0F, 0F);
        static readonly Vector3 rightVector = new Vector3(1F, 0F, 0F);
        static readonly Vector3 forwardVector = new Vector3(0F, 0F, 1F);
        static readonly Vector3 backVector = new Vector3(0F, 0F, -1F);
        static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public static Vector3 zero { get { return zeroVector; } }
        public static Vector3 one { get { return oneVector; } }
        public static Vector3 forward { get { return forwardVector; } }
        public static Vector3 back { get { return backVector; } }
        public static Vector3 up { get { return upVector; } }
        public static Vector3 down { get { return downVector; } }
        public static Vector3 left { get { return leftVector; } }
        public static Vector3 right { get { return rightVector; } }
        public static Vector3 positiveInfinity { get { return positiveInfinityVector; } }
        public static Vector3 negativeInfinity { get { return negativeInfinityVector; } }


        public static float Clamp01(float value)
        {
            if (value < 0.0) return 0.0f;
            return value > 1.0f ? 1.0f : value;
        }

        public static float Clamp(float value, float a, float b)
        {
            if (value < a) return a;
            return value > b ? b : value;
        }

        internal static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Clamp01(t);
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        public static float Magnitude(Vector3 vector) { return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        public static Vector3 Normalize(Vector3 value)
        {
            float mag = Magnitude(value);
            if (mag > kEpsilon)
                return value / mag;
            else
                return zero;
        }

        public void Normalize()
        {
            float mag = Magnitude(this);
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vector3 normalized
        {
            get { return Vector3.Normalize(this); }
        }

        public static float Dot(Vector3 lhs, Vector3 rhs) { return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z; }

        public float sqrMagnitude { get { return x * x + y * y + z * z; } }

        public static float Angle(Vector3 from, Vector3 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Clamp(Dot(from, to) / denominator, -1F, 1F);
            return ((float)Math.Acos(dot)) * 57.29577951f;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
        public static Vector3 operator *(Vector3 a, float d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static Vector3 operator *(float d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static Vector3 operator /(Vector3 a, float d) { return new Vector3(a.x / d, a.y / d, a.z / d); }
    }

    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        private float v;

        public Quaternion(float x, float y, float z, float w) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static (Vector3, float) QuatToAxisAngle(Quaternion q)
        {
            var axis = new Vector3();
            float angle;

            // Normalize quaternion
            q = NormalizeQuaternion(q);

            // Calculate rotation angle
            angle = 2 * (float)Math.Acos(q.w);

            // Calculate rotation axis
            float s = (float)Math.Sqrt(1 - q.w * q.w);
            if (s < 1e-8)
            {
                axis = Vector3.right;
            }
            else
            {
                axis = new Vector3(q.x / s, q.y / s, q.z / s);
            }

            return (axis.normalized, angle);
        }

        public static Quaternion AxisAngleToQuat(Vector3 axis, float angle)
        {
            // Calculate sine and cosine of half angle
            var s = (float)Math.Sin(angle / 2);
            var c = (float)Math.Cos(angle / 2);

            // Create quaternion
            return new Quaternion(axis.x * s, axis.y * s, axis.z * s, c);
        }

        private static Quaternion NormalizeQuaternion(Quaternion q)
        {
            var magnitude = (float)Math.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (magnitude > 0)
            {
                q.x /= magnitude;
                q.y /= magnitude;
                q.z /= magnitude;
                q.w /= magnitude;
            }
            return q;
        }


        private static Vector3 QuaternionToEuler(Quaternion q)
        {
            var pitch = (float)Math.Asin(2 * (q.w * q.x - q.y * q.z));
            var yaw = (float)Math.Atan2(2 * (q.w * q.y + q.z * q.x), 1 - 2 * (q.y * q.y + q.x * q.x));
            var roll = (float)Math.Atan2(2 * (q.w * q.z + q.x * q.y), 1 - 2 * (q.z * q.z + q.x * q.x));

            return new Vector3(pitch, yaw, roll);
        }

        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            float x = rotation.x * 2F;
            float y = rotation.y * 2F;
            float z = rotation.z * 2F;
            float xx = rotation.x * x;
            float yy = rotation.y * y;
            float zz = rotation.z * z;
            float xy = rotation.x * y;
            float xz = rotation.x * z;
            float yz = rotation.y * z;
            float wx = rotation.w * x;
            float wy = rotation.w * y;
            float wz = rotation.w * z;

            Vector3 res;
            res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
            return res;
        }

        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        internal static Quaternion Lerp(Quaternion a, Quaternion b, float t)
        {
            t = Vector3.Clamp01(t);
            return new Quaternion(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            );
        }

        public static Quaternion AngleAxis(float aAngle, Vector3 aAxis)
        {
            aAxis.Normalize();
            float rad = aAngle * 57.29577951f * 0.5f;
            aAxis = aAxis * (float)Math.Sin(rad);
            return new Quaternion(aAxis.x, aAxis.y, aAxis.z, (float)Math.Cos(rad));
        }

        public static Quaternion FromToRotation(Vector3 aFrom, Vector3 aTo)
        {
            Vector3 axis = Vector3.Cross(aFrom, aTo);
            float angle = Vector3.Angle(aFrom, aTo);
            return Quaternion.AngleAxis(angle, axis.normalized);
        }
    }

    public class Transform
    {
        public Vector3 position;
        public Quaternion rotation;

        public List<float> ToArray()
        {
            //var (axis, angle) = Quaternion.QuatToAxisAngle(rotation);
            return new List<float> { position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w };
        }
    }

    public static class ReplayEncoder
    {
        public static ReplayOffsets Encode(Replay replay, BinaryWriter stream)
        {
            stream.Write(0x442d3d69);
            stream.Write((byte)1);

            var offsets = new ReplayOffsets();

            for (int a = 0; a < ((int)StructType.pauses) + 1; a++)
            {
                StructType type = (StructType)a;
                stream.Write((byte)a);

                switch (type)
                {
                    case StructType.info:
                        EncodeInfo(replay.info, stream);
                        break;
                    case StructType.frames:
                        offsets.Frames = (int)stream.BaseStream.Position;
                        EncodeFrames(replay.frames, stream);
                        break;
                    case StructType.notes:
                        offsets.Notes = (int)stream.BaseStream.Position;
                        EncodeNotes(replay.notes, stream);
                        break;
                    case StructType.walls:
                        offsets.Walls = (int)stream.BaseStream.Position;
                        EncodeWalls(replay.walls, stream);
                        break;
                    case StructType.heights:
                        offsets.Heights = (int)stream.BaseStream.Position;
                        EncodeHeights(replay.heights, stream);
                        break;
                    case StructType.pauses:
                        offsets.Pauses = (int)stream.BaseStream.Position;
                        EncodePauses(replay.pauses, stream);
                        break;
                    case StructType.saberOffsets:
                        offsets.SaberOffsets = (int)stream.BaseStream.Position;
                        EncodeSaberOffsets(replay.saberOffsets, stream);
                        break;
                    case StructType.customData:
                        offsets.CustomData = (int)stream.BaseStream.Position;
                        EncodeCustomData(replay.customData, stream);
                        break;
                }
            }

            return offsets;
        }

        public static void EncodeInfo(ReplayInfo info, BinaryWriter stream)
        {
            EncodeString(info.version, stream);
            EncodeString(info.gameVersion, stream);
            EncodeString(info.timestamp, stream);

            EncodeString(info.playerID, stream);
            EncodeString(info.playerName, stream);
            EncodeString(info.platform, stream);

            EncodeString(info.trackingSytem, stream);
            EncodeString(info.hmd, stream);
            EncodeString(info.controller, stream);

            EncodeString(info.hash, stream);
            EncodeString(info.songName, stream);
            EncodeString(info.mapper, stream);
            EncodeString(info.difficulty, stream);

            stream.Write(info.score);
            EncodeString(info.mode, stream);
            EncodeString(info.environment, stream);
            EncodeString(info.modifiers, stream);
            stream.Write(info.jumpDistance);
            stream.Write(info.leftHanded);
            stream.Write(info.height);

            stream.Write(info.startTime);
            stream.Write(info.failTime);
            stream.Write(info.speed);
        }

        public static void EncodeFrames(List<Frame> frames, BinaryWriter stream)
        {
            stream.Write((uint)frames.Count);
            foreach (var frame in frames)
            {
                stream.Write(frame.time);
                stream.Write(frame.fps);
                EncodeVector(frame.head.position, stream);
                EncodeQuaternion(frame.head.rotation, stream);
                EncodeVector(frame.leftHand.position, stream);
                EncodeQuaternion(frame.leftHand.rotation, stream);
                EncodeVector(frame.rightHand.position, stream);
                EncodeQuaternion(frame.rightHand.rotation, stream);
            }
        }

        public static void EncodeNotes(List<NoteEvent> notes, BinaryWriter stream)
        {
            stream.Write((uint)notes.Count);
            foreach (var note in notes)
            {
                stream.Write(note.noteID);
                stream.Write(note.eventTime);
                stream.Write(note.spawnTime);
                stream.Write((int)note.eventType);
                if (note.eventType == NoteEventType.good || note.eventType == NoteEventType.bad) {
                    EncodeNoteInfo(note.noteCutInfo, stream);
                }
            }
        }

        public static void EncodeWalls(List<WallEvent> walls, BinaryWriter stream)
        {
            stream.Write((uint)walls.Count);
            foreach (var wall in walls)
            {
                stream.Write(wall.wallID);
                stream.Write(wall.energy);
                stream.Write(wall.time);
                stream.Write(wall.spawnTime);
            }
        }

        public static void EncodeHeights(List<AutomaticHeight> heights, BinaryWriter stream)
        {
            stream.Write((uint)heights.Count);
            foreach (var height in heights)
            {
                stream.Write(height.height);
                stream.Write(height.time);
            }
        }

        public static void EncodePauses(List<Pause> pauses, BinaryWriter stream)
        {
            stream.Write((uint)pauses.Count);
            foreach (var pause in pauses)
            {
                stream.Write(pause.duration);
                stream.Write(pause.time);
            }
        }

        static void EncodeSaberOffsets(SaberOffsets saberOffsets, BinaryWriter stream)
        {
            EncodeVector(saberOffsets.leftSaberLocalPosition, stream);
            EncodeQuaternion(saberOffsets.leftSaberLocalRotation, stream);
            EncodeVector(saberOffsets.rightSaberLocalPosition, stream);
            EncodeQuaternion(saberOffsets.rightSaberLocalRotation, stream);
        }

        static void EncodeCustomData(Dictionary<string, byte[]> customData, BinaryWriter stream)
        {
            stream.Write(customData.Count);
            foreach (var pair in customData) {
                EncodeString(pair.Key, stream);
                EncodeByteArray(pair.Value, stream);
            }
        }

        static void EncodeNoteInfo(NoteCutInfo info, BinaryWriter stream)
        {
            stream.Write(info.speedOK);
            stream.Write(info.directionOK);
            stream.Write(info.saberTypeOK);
            stream.Write(info.wasCutTooSoon);
            stream.Write(info.saberSpeed);
            EncodeVector(info.saberDir, stream);
            stream.Write(info.saberType);
            stream.Write(info.timeDeviation);
            stream.Write(info.cutDirDeviation);
            EncodeVector(info.cutPoint, stream);
            EncodeVector(info.cutNormal, stream);
            stream.Write(info.cutDistanceToCenter);
            stream.Write(info.cutAngle);
            stream.Write(info.beforeCutRating);
            stream.Write(info.afterCutRating);
        }

        static void EncodeString(string value, BinaryWriter stream)
        {
            string toEncode = value != null ? value : "";
            stream.Write((int)toEncode.Length);
            stream.Write(Encoding.UTF8.GetBytes(toEncode));
        }

        static void EncodeVector(Vector3 vector, BinaryWriter stream)
        {
            stream.Write(vector.x);
            stream.Write(vector.y);
            stream.Write(vector.z);
        }

        static void EncodeQuaternion(Quaternion quaternion, BinaryWriter stream)
        {
            stream.Write(quaternion.x);
            stream.Write(quaternion.y);
            stream.Write(quaternion.z);
            stream.Write(quaternion.w);
        }

        static void EncodeByteArray(byte[] value, BinaryWriter stream)
        {
            stream.Write(value.Length);
            stream.Write(value);
        }
    }

    public class AsyncReplayDecoder
    {
        public Replay replay = new Replay();
        public ReplayOffsets offsets = new ReplayOffsets();

        int offset = 0;
        public byte[] replayData = new byte[1024000];

        private Stream stream;

        public async Task<(ReplayInfo?, Task<Replay?>?)> StartDecodingStream(Stream stream)
        {
            int magic = await DecodeInt(stream);
            byte version = await DecodeByte(stream);

            if (magic == 0x442d3d69 && version == 1)
            {
                StructType type = (StructType)await DecodeByte(stream);
                if (type == StructType.info) {
                    replay.info = await DecodeInfo(stream);
                    this.stream = stream;
                    return (replay.info, ContinueDecoding());
                } else {
                    return (null, null);
                }
            }
            else
            {
                return (null, null);
            }
        }

        public async Task<ReplayInfo?> DecodeInfoOnly(Stream stream)
        {
            int magic = await DecodeInt(stream);
            byte version = await DecodeByte(stream);

            if (magic == 0x442d3d69 && version == 1)
            {
                StructType type = (StructType)await DecodeByte(stream);
                if (type == StructType.info) {
                    replay.info = await DecodeInfo(stream);
                    this.stream = stream;
                    return replay.info;
                } else {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async Task<Replay?> ContinueDecoding() 
        {
            for (int a = (int)StructType.frames; a < ((int)StructType.pauses) + 1; a++) {
                StructType type = (StructType)await DecodeByte(stream);

                switch (type)
                {
                    case StructType.frames:
                        offsets.Frames = offset;
                        replay.frames = await DecodeFrames(stream);
                        break;
                    case StructType.notes:
                        offsets.Notes = offset;
                        replay.notes = await DecodeNotes(stream);
                        break;
                    case StructType.walls:
                        offsets.Walls = offset;
                        replay.walls = await DecodeWalls(stream);
                        break;
                    case StructType.heights:
                        offsets.Heights = offset;
                        replay.heights = await DecodeHeight(stream);
                        break;
                    case StructType.pauses:
                        offsets.Pauses = offset;
                        replay.pauses = await DecodePauses(stream);
                        break;
                }
            }

            Array.Resize(ref replayData, offset);

            return replay;
        }

        private async Task<ReplayInfo> DecodeInfo(Stream stream)
        {
                ReplayInfo result = new ReplayInfo();

                result.version = await DecodeString(stream);
                result.gameVersion = await DecodeString(stream);
                result.timestamp = await DecodeString(stream);

                result.playerID = await DecodeString(stream);
                result.playerName = await DecodeString(stream);
                result.platform = await DecodeString(stream);

                result.trackingSytem = await DecodeString(stream);
                result.hmd = await DecodeString(stream);
                result.controller = await DecodeString(stream);

                result.hash = await DecodeString(stream);
                result.songName = await DecodeString(stream);
                result.mapper = await DecodeString(stream);
                result.difficulty = await DecodeString(stream);
                
                result.score = await DecodeInt(stream);
                result.mode = await DecodeString(stream);
                result.environment = await DecodeString(stream);
                result.modifiers = await DecodeString(stream);
                result.jumpDistance = await DecodeFloat(stream);
                result.leftHanded = await DecodeBool(stream);
                result.height = await DecodeFloat(stream);

                result.startTime = await DecodeFloat(stream);
                result.failTime = await DecodeFloat(stream);
                result.speed = await DecodeFloat(stream);

                return result;
        }

        private async Task<List<Frame>> DecodeFrames(Stream stream)
        {
            int length = await DecodeInt(stream);
            List<Frame> result = new List<Frame>();
            for (int i = 0; i < length; i++)
            {
                result.Add(await DecodeFrame(stream));
            }
            return result;
        }

        private async Task<Frame> DecodeFrame(Stream stream)
        {
            Frame result = new Frame();
            result.time = await DecodeFloat(stream);
            result.fps = await DecodeInt(stream);
            result.head = await DecodeEuler(stream);
            result.leftHand = await DecodeEuler(stream);
            result.rightHand = await DecodeEuler(stream);

            return result;
        }

        public async Task<List<NoteEvent>> DecodeNotes(Stream stream)
        {
            int length = await DecodeInt(stream);
            List<NoteEvent> result = new List<NoteEvent>();
            for (int i = 0; i < length; i++)
            {
                result.Add(await DecodeNote(stream));
            }
            return result;
        }

        public async Task<List<WallEvent>> DecodeWalls(Stream stream)
        {
            int length = await DecodeInt(stream);
            List<WallEvent> result = new List<WallEvent>();
            for (int i = 0; i < length; i++)
            {
                WallEvent wall = new WallEvent();
                wall.wallID = await DecodeInt(stream);
                wall.energy = await DecodeFloat(stream);
                wall.time = await DecodeFloat(stream);
                wall.spawnTime = await DecodeFloat(stream);
                result.Add(wall);
            }
            return result;
        }

        private async Task<List<AutomaticHeight>> DecodeHeight(Stream stream)
        {
            int length = await DecodeInt(stream);
            List<AutomaticHeight> result = new List<AutomaticHeight>();
            for (int i = 0; i < length; i++)
            {
                AutomaticHeight height = new AutomaticHeight();
                height.height = await DecodeFloat(stream);
                height.time = await DecodeFloat(stream);
                result.Add(height);
            }
            return result;
        }

        private async Task<List<Pause>> DecodePauses(Stream stream)
        {
            int length = await DecodeInt(stream);
            List<Pause> result = new List<Pause>();
            for (int i = 0; i < length; i++)
            {
                Pause pause = new Pause();
                pause.duration = await DecodeLong(stream);
                pause.time = await DecodeFloat(stream);
                result.Add(pause);
            }
            return result;
        }
        private async Task<NoteEvent> DecodeNote(Stream stream)
        {
            NoteEvent result = new NoteEvent();
            result.noteID = await DecodeInt(stream);
            result.eventTime = await DecodeFloat(stream);
            result.spawnTime = await DecodeFloat(stream);
            result.eventType = (NoteEventType) await DecodeInt(stream);
            if (result.eventType == NoteEventType.good || result.eventType == NoteEventType.bad) {
                result.noteCutInfo = await DecodeCutInfo(stream);
            }

            if (result.noteID == -1 || (result.noteID > 0 && result.noteID < 100000 && result.noteID % 10 == 9)) {
                result.noteID += 4;
                result.eventType = NoteEventType.bomb;
            }

            return result;
        }

        private async Task<NoteCutInfo> DecodeCutInfo(Stream stream)
        {
            NoteCutInfo result = new NoteCutInfo();
            result.speedOK = await DecodeBool(stream);
            result.directionOK = await DecodeBool(stream);
            result.saberTypeOK = await DecodeBool(stream);
            result.wasCutTooSoon = await DecodeBool(stream);
            result.saberSpeed = await DecodeFloat(stream);
            result.saberDir = await DecodeVector3(stream);
            result.saberType = await DecodeInt(stream);
            result.timeDeviation = await DecodeFloat(stream);
            result.cutDirDeviation = await DecodeFloat(stream);
            result.cutPoint = await DecodeVector3(stream);
            result.cutNormal = await DecodeVector3(stream);
            result.cutDistanceToCenter = await DecodeFloat(stream);
            result.cutAngle = await DecodeFloat(stream);
            result.beforeCutRating = await DecodeFloat(stream);
            result.afterCutRating = await DecodeFloat(stream);
            return result;
        }

        private async Task<Transform> DecodeEuler(Stream stream)
        {
            Transform result = new Transform();
            result.position = await DecodeVector3(stream);
            result.rotation = await DecodeQuaternion(stream);

            return result;
        }

        private async Task<Vector3> DecodeVector3(Stream stream)
        {
            Vector3 result = new Vector3();
            result.x = await DecodeFloat(stream);
            result.y = await DecodeFloat(stream);
            result.z = await DecodeFloat(stream);

            return result;
        }

        private async Task<Quaternion> DecodeQuaternion(Stream stream)
        {
            Quaternion result = new Quaternion();
            result.x = await DecodeFloat(stream);
            result.y = await DecodeFloat(stream);
            result.z = await DecodeFloat(stream);
            result.w = await DecodeFloat(stream);

            return result;
        }

        private void EnsureBufferSize(int size) {
            if (offset + size > replayData.Length) {
                Array.Resize(ref replayData, replayData.Length * 2);
            }
        }

        private async Task<long> DecodeLong(Stream stream)
        {
            EnsureBufferSize(8);
            await stream.ReadAsync(replayData, offset, 8);
            offset += 8;
            return BitConverter.ToInt64(replayData, offset - 8);
        }

        private async Task<int> DecodeInt(Stream stream)
        {
            EnsureBufferSize(4);
            await stream.ReadAsync(replayData, offset, 4);
            offset += 4;
            return BitConverter.ToInt32(replayData, offset - 4);
        }

        public async Task<byte> DecodeByte(Stream stream)
        {
            EnsureBufferSize(1);
            await stream.ReadAsync(replayData, offset, 1);
            offset++;
            return replayData[offset - 1];
        }

        private async Task<string> DecodeString(Stream stream, int size = 4)
        {
            EnsureBufferSize(size);
            await stream.ReadAsync(replayData, offset, size);
            offset += size;
            int length = BitConverter.ToInt32(replayData, offset - 4);

            if (length > 1000 || length < 0)
            {
                return await DecodeString(stream, 1);
            }

            EnsureBufferSize(length);
            await stream.ReadAsync(replayData, offset, length);
            string @string = Encoding.UTF8.GetString(replayData, offset, length);
            offset += length;
            return @string;
        }

        private async Task<float> DecodeFloat(Stream stream)
        {
            EnsureBufferSize(4);
            await stream.ReadAsync(replayData, offset, 4);
            offset += 4;
            return BitConverter.ToSingle(replayData, offset - 4);
        }

        private async Task<bool> DecodeBool(Stream stream)
        {
            EnsureBufferSize(1);
            await stream.ReadAsync(replayData, offset, 1);
            offset++;
            return BitConverter.ToBoolean(replayData, offset - 1);
        }
    }

    public static class ReplayDecoder
    {
        public static (Replay?, ReplayOffsets?) Decode(byte[] buffer)
        {
            int arrayLength = (int)buffer.Length;

            int pointer = 0;

            int magic = DecodeInt(buffer, ref pointer);
            byte version = buffer[pointer++];

            if (magic == 0x442d3d69 && version == 1)
            {
                Replay replay = new Replay();
                ReplayOffsets offsets = new ReplayOffsets();

                for (int a = 0; a < ((int)StructType.pauses) + 1 && pointer < arrayLength; a++)
                {
                    StructType type = (StructType)buffer[pointer++];

                    switch (type)
                    {
                        case StructType.info:
                            replay.info = DecodeInfo(buffer, ref pointer);
                            break;
                        case StructType.frames:
                            offsets.Frames = pointer;
                            replay.frames = DecodeFrames(buffer, ref pointer);
                            break;
                        case StructType.notes:
                            offsets.Notes = pointer;
                            replay.notes = DecodeNotes(buffer, ref pointer);
                            break;
                        case StructType.walls:
                            offsets.Walls = pointer;
                            replay.walls = DecodeWalls(buffer, ref pointer);
                            break;
                        case StructType.heights:
                            offsets.Heights = pointer;
                            replay.heights = DecodeHeights(buffer, ref pointer);
                            break;
                        case StructType.pauses:
                            offsets.Pauses = pointer;
                            replay.pauses = DecodePauses(buffer, ref pointer);
                            break;
                        case StructType.saberOffsets:
                            offsets.SaberOffsets = pointer;
                            replay.saberOffsets = DecodeSaberOffsets(buffer, ref pointer);
                            break;
                        case StructType.customData:
                            offsets.CustomData = pointer;
                            replay.customData = DecodeCustomData(buffer, ref pointer);
                            break;
                        }
                }

                return (replay, offsets);
            }
            else
            {
                return (null, null);
            }
        }

        public static ReplayInfo DecodeInfo(byte[] buffer, ref int pointer)
        {
                ReplayInfo result = new ReplayInfo();

                result.version = DecodeString(buffer, ref pointer);
                result.gameVersion = DecodeString(buffer, ref pointer);
                result.timestamp = DecodeString(buffer, ref pointer);

                result.playerID = DecodeString(buffer, ref pointer);
                result.playerName = DecodeName(buffer, ref pointer);
                result.platform = DecodeString(buffer, ref pointer);

                result.trackingSytem = DecodeString(buffer, ref pointer);
                result.hmd = DecodeString(buffer, ref pointer);
                result.controller = DecodeString(buffer, ref pointer);

                result.hash = DecodeString(buffer, ref pointer);
                result.songName = DecodeString(buffer, ref pointer);
                result.mapper = DecodeString(buffer, ref pointer);
                result.difficulty = DecodeString(buffer, ref pointer);
                
                result.score = DecodeInt(buffer, ref pointer);
                result.mode = DecodeString(buffer, ref pointer);
                result.environment = DecodeString(buffer, ref pointer);
                result.modifiers = DecodeString(buffer, ref pointer);
                result.jumpDistance = DecodeFloat(buffer, ref pointer);
                result.leftHanded = DecodeBool(buffer, ref pointer);
                result.height = DecodeFloat(buffer, ref pointer);

                result.startTime = DecodeFloat(buffer, ref pointer);
                result.failTime = DecodeFloat(buffer, ref pointer);
                result.speed = DecodeFloat(buffer, ref pointer);

                return result;
         }

        public static List<Frame> DecodeFrames(byte[] buffer, ref int pointer)
        {
            int length = DecodeInt(buffer, ref pointer);
            List<Frame> result = new List<Frame>();
            for (int i = 0; i < length; i++)
            {
                var frame  = DecodeFrame(buffer, ref pointer);
                if (frame.time != 0 && (result.Count == 0 || frame.time != result[result.Count - 1].time)) {
                    result.Add(frame);
                }
            }
            return result;
        }

        public static Frame DecodeFrame(byte[] buffer, ref int pointer)
        {
            Frame result = new Frame();
            result.time = DecodeFloat(buffer, ref pointer);
            result.fps = DecodeInt(buffer, ref pointer);
            result.head = DecodeEuler(buffer, ref pointer);
            result.leftHand = DecodeEuler(buffer, ref pointer);
            result.rightHand = DecodeEuler(buffer, ref pointer);

            return result;
        }

        public static List<NoteEvent> DecodeNotes(byte[] buffer, ref int pointer)
        {
            int length = DecodeInt(buffer, ref pointer);
            List<NoteEvent> result = new List<NoteEvent>();
            for (int i = 0; i < length; i++)
            {
                result.Add(DecodeNote(buffer, ref pointer));
            }
            return result;
        }

        public static List<WallEvent> DecodeWalls(byte[] buffer, ref int pointer)
        {
            int length = DecodeInt(buffer, ref pointer);
            List<WallEvent> result = new List<WallEvent>();
            for (int i = 0; i < length; i++)
            {
                WallEvent wall = new WallEvent();
                wall.wallID = DecodeInt(buffer, ref pointer);
                wall.energy = DecodeFloat(buffer, ref pointer);
                wall.time = DecodeFloat(buffer, ref pointer);
                wall.spawnTime = DecodeFloat(buffer, ref pointer);
                result.Add(wall);
            }
            return result;
        }

        public static List<AutomaticHeight> DecodeHeights(byte[] buffer, ref int pointer)
        {
            int length = DecodeInt(buffer, ref pointer);
            List<AutomaticHeight> result = new List<AutomaticHeight>();
            for (int i = 0; i < length; i++)
            {
                AutomaticHeight height = new AutomaticHeight();
                height.height = DecodeFloat(buffer, ref pointer);
                height.time = DecodeFloat(buffer, ref pointer);
                result.Add(height);
            }
            return result;
        }

        public static List<Pause> DecodePauses(byte[] buffer, ref int pointer)
        {
            int length = DecodeInt(buffer, ref pointer);
            List<Pause> result = new List<Pause>();
            for (int i = 0; i < length; i++)
            {
                Pause pause = new Pause();
                pause.duration = DecodeLong(buffer, ref pointer);
                pause.time = DecodeFloat(buffer, ref pointer);
                result.Add(pause);
            }
            return result;
        }

        private static SaberOffsets DecodeSaberOffsets(byte[] buffer, ref int pointer)
        {
            var result = new SaberOffsets();
            result.leftSaberLocalPosition = DecodeVector3(buffer, ref pointer);
            result.leftSaberLocalRotation = DecodeQuaternion(buffer, ref pointer);
            result.rightSaberLocalPosition = DecodeVector3(buffer, ref pointer);
            result.rightSaberLocalRotation = DecodeQuaternion(buffer, ref pointer);
            return result;
        }
        
        private static Dictionary<string, byte[]> DecodeCustomData(byte[] buffer, ref int pointer)
        {
            var result = new Dictionary<string, byte[]>();
            var count = DecodeInt(buffer, ref pointer);
            for (var i = 0; i < count; i++) {
                var key = DecodeString(buffer, ref pointer);
                var value = DecodeByteArray(buffer, ref pointer);
                result[key] = value;
            }
            return result;
        }

        private static NoteEvent DecodeNote(byte[] buffer, ref int pointer)
        {
            NoteEvent result = new NoteEvent();
            result.noteID = DecodeInt(buffer, ref pointer);
            result.eventTime = DecodeFloat(buffer, ref pointer);
            result.spawnTime = DecodeFloat(buffer, ref pointer);
            result.eventType = (NoteEventType)DecodeInt(buffer, ref pointer);
            if (result.eventType == NoteEventType.good || result.eventType == NoteEventType.bad) {
                result.noteCutInfo = DecodeCutInfo(buffer, ref pointer);
            }

            if (result.noteID == -1 || (result.noteID > 0 && result.noteID < 100000 && result.noteID % 10 == 9)) {
                result.noteID += 4;
                result.eventType = NoteEventType.bomb;
            }

            return result;
        }

        private static NoteCutInfo DecodeCutInfo(byte[] buffer, ref int pointer)
        {
            NoteCutInfo result = new NoteCutInfo();
            result.speedOK = DecodeBool(buffer, ref pointer);
            result.directionOK = DecodeBool(buffer, ref pointer);
            result.saberTypeOK = DecodeBool(buffer, ref pointer);
            result.wasCutTooSoon = DecodeBool(buffer, ref pointer);
            result.saberSpeed = DecodeFloat(buffer, ref pointer);
            result.saberDir = DecodeVector3(buffer, ref pointer);
            result.saberType = DecodeInt(buffer, ref pointer);
            result.timeDeviation = DecodeFloat(buffer, ref pointer);
            result.cutDirDeviation = DecodeFloat(buffer, ref pointer);
            result.cutPoint = DecodeVector3(buffer, ref pointer);
            result.cutNormal = DecodeVector3(buffer, ref pointer);
            result.cutDistanceToCenter = DecodeFloat(buffer, ref pointer);
            result.cutAngle = DecodeFloat(buffer, ref pointer);
            result.beforeCutRating = DecodeFloat(buffer, ref pointer);
            result.afterCutRating = DecodeFloat(buffer, ref pointer);
            return result;
        }

        private static Transform DecodeEuler(byte[] buffer, ref int pointer)
        {
            Transform result = new Transform();
            result.position = DecodeVector3(buffer, ref pointer);
            result.rotation = DecodeQuaternion(buffer, ref pointer);

            return result;
        }

        private static Vector3 DecodeVector3(byte[] buffer, ref int pointer)
        {
            Vector3 result = new Vector3();
            result.x = DecodeFloat(buffer, ref pointer);
            result.y = DecodeFloat(buffer, ref pointer);
            result.z = DecodeFloat(buffer, ref pointer);

            return result;
        }

        private static Quaternion DecodeQuaternion(byte[] buffer, ref int pointer)
        {
            Quaternion result = new Quaternion();
            result.x = DecodeFloat(buffer, ref pointer);
            result.y = DecodeFloat(buffer, ref pointer);
            result.z = DecodeFloat(buffer, ref pointer);
            result.w = DecodeFloat(buffer, ref pointer);

            return result;
        }

        private static long DecodeLong(byte[] buffer, ref int pointer)
        {
            long result = BitConverter.ToInt64(buffer, pointer);
            pointer += 8;
            return result;
        }

        private static int DecodeInt(byte[] buffer, ref int pointer)
        {
            int result = BitConverter.ToInt32(buffer, pointer);
            pointer += 4;
            return result;
        }

        private static string DecodeName(byte[] buffer, ref int pointer)
        {
            int length = BitConverter.ToInt32(buffer, pointer);
            int lengthOffset = 0;
            if (length > 0)
            {
                while (BitConverter.ToInt32(buffer, length + pointer + 4 + lengthOffset) != 6 
                    && BitConverter.ToInt32(buffer, length + pointer + 4 + lengthOffset) != 5 
                    && BitConverter.ToInt32(buffer, length + pointer + 4 + lengthOffset) != 8)
                {
                    lengthOffset++;
                }
            }
            string @string = Encoding.UTF8.GetString(buffer, pointer + 4, length + lengthOffset);
            pointer += length + 4 + lengthOffset;
            return @string;
        }

        private static string DecodeString(byte[] buffer, ref int pointer)
        {
            int length = BitConverter.ToInt32(buffer, pointer);
            if (length > 300 || length < 0)
            {
                pointer += 1;
                return DecodeString(buffer, ref pointer);
            }
            string @string = Encoding.UTF8.GetString(buffer, pointer + 4, length);
            pointer += length + 4;
            return @string;
        }

        private static float DecodeFloat(byte[] buffer, ref int pointer)
        {
            float result = BitConverter.ToSingle(buffer, pointer);
            pointer += 4;
            return result;
        }

        private static bool DecodeBool(byte[] buffer, ref int pointer)
        {
            bool result = BitConverter.ToBoolean(buffer, pointer);
            pointer++;
            return result;
        }

        private static byte[] DecodeByteArray(byte[] buffer, ref int pointer) {
            var count = DecodeInt(buffer, ref pointer);
            var result = new byte[count];
            Array.Copy(buffer, pointer, result, 0, count);
            pointer += count;
            return result;
        }
    }

    public class NoteScore
    {
        public int pre_score;
        public int post_score;
        public int acc_score;
        public int value;

        public NoteScore(int pre_score, int post_score, int acc_score)
        {
            this.pre_score = pre_score;
            this.post_score = post_score;
            this.acc_score = acc_score;
            value = pre_score + post_score + acc_score;
        }

        public static NoteScore CalculateNoteScore(NoteCutInfo cut, ScoringType scoring_type)
        {
            if (!cut.directionOK || !cut.saberTypeOK || !cut.speedOK)
            {
                return new NoteScore(0, 0, 0);
            }

            int before_cut_raw_score = 0;

            if (scoring_type != ScoringType.BurstSliderElement)
            {
                if (scoring_type == ScoringType.SliderTail)
                {
                    before_cut_raw_score = 70;
                }
                else
                {
                    before_cut_raw_score = (int)(70 * cut.beforeCutRating);
                    before_cut_raw_score = RoundHalfUp(before_cut_raw_score);
                    before_cut_raw_score = Clamp(before_cut_raw_score, 0, 70);
                }
            }

            int after_cut_raw_score = 0;

            if (scoring_type != ScoringType.BurstSliderElement)
            {
                if (scoring_type == ScoringType.BurstSliderHead)
                {
                    after_cut_raw_score = 0;
                }
                else if (scoring_type == ScoringType.SliderHead)
                {
                    after_cut_raw_score = 30;
                }
                else
                {
                    after_cut_raw_score = (int)(30 * cut.afterCutRating);
                    after_cut_raw_score = RoundHalfUp(after_cut_raw_score);
                    after_cut_raw_score = Clamp(after_cut_raw_score, 0, 30);
                }
            }

            int cut_distance_raw_score;

            if (scoring_type == ScoringType.BurstSliderElement)
            {
                cut_distance_raw_score = 20;
            }
            else
            {
                var cut_distance_raw_score_float = (15f * (1f - Clamp(cut.cutDistanceToCenter / 0.3f, 0f, 1f)));
                cut_distance_raw_score = RoundHalfUp(cut_distance_raw_score_float);
            }

            return new NoteScore(before_cut_raw_score, after_cut_raw_score, cut_distance_raw_score);
        }

        private static int RoundHalfUp(float value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Clamp(value, min, max);
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Clamp(value, min, max);
        }
    }
}

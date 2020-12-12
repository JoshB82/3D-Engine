﻿namespace _3D_Engine
{
    public partial class Group : SceneObject
    {
        #region Rotations

        public override void Set_Direction_1(Vector3D new_world_direction_forward, Vector3D new_world_direction_up)
        {
            base.Set_Direction_1(new_world_direction_forward, new_world_direction_up);

            if (Scene_Objects is null || Scene_Objects.Count == 0) return;

            // Calculate rotation matrices
            Matrix4x4 direction_forward_rotation = Transform.Rotate_Between_Vectors(World_Direction_Forward, new_world_direction_forward);
            Matrix4x4 direction_up_rotation = Transform.Rotate_Between_Vectors((Vector3D) (direction_forward_rotation * new Vector4D(World_Direction_Up, 1)), new_world_direction_up);
            Matrix4x4 resultant = direction_up_rotation * direction_forward_rotation;

            // Apply rotation matrices to children of group
            foreach (SceneObject scene_object in Scene_Objects)
            {
                scene_object.Set_Direction_1
                (
                    (Vector3D)(direction_forward_rotation * scene_object.World_Direction_Forward),
                    (Vector3D)(resultant * new Vector4D(scene_object.World_Direction_Up, 1))
                );
            }
        }
        public override void Set_Direction_2(Vector3D new_world_direction_up, Vector3D new_world_direction_right)
        {
            base.Set_Direction_2(new_world_direction_up, new_world_direction_right);

            if (Scene_Objects.Count == 0) return;

            // Calculate rotation matrices
            Matrix4x4 direction_up_rotation = Transform.Rotate_Between_Vectors(World_Direction_Up, new_world_direction_up);
            Matrix4x4 direction_right_rotation = Transform.Rotate_Between_Vectors((Vector3D) (direction_up_rotation * new Vector4D(World_Direction_Right, 1)), new_world_direction_right);
            Matrix4x4 resultant = direction_right_rotation * direction_up_rotation;

            // Apply rotation matrices to children of group
            foreach (SceneObject scene_object in Scene_Objects)
            {
                scene_object.Set_Direction_2
                (
                    (Vector3D)(direction_up_rotation * scene_object.World_Direction_Up),
                    (Vector3D)(resultant * new Vector4D(scene_object.World_Direction_Right, 1))
                );
            }
        }
        public override void Set_Direction_3(Vector3D new_world_direction_right, Vector3D new_world_direction_forward)
        {
            base.Set_Direction_3(new_world_direction_right, new_world_direction_forward);

            if (Scene_Objects.Count == 0) return;

            // Calculate rotation matrices
            Matrix4x4 direction_right_rotation = Transform.Rotate_Between_Vectors(World_Direction_Right, new_world_direction_right);
            Matrix4x4 direction_forward_rotation = Transform.Rotate_Between_Vectors((Vector3D) (direction_right_rotation * new Vector4D(World_Direction_Forward, 1)),
                new_world_direction_forward);
            Matrix4x4 resultant = direction_forward_rotation * direction_right_rotation;

            // Apply rotation matrices to children of group
            foreach (SceneObject scene_object in Scene_Objects)
            {
                scene_object.Set_Direction_3
                (
                    (Vector3D)(direction_right_rotation * scene_object.World_Direction_Right),
                    (Vector3D)(resultant * new Vector4D(scene_object.World_Direction_Forward,1))
                );
            }
        }

        #endregion

        #region Translations

        public override void Translate_X(float distance)
        {
            base.Translate_X(distance);
            foreach (SceneObject scene_object in Scene_Objects) scene_object.Translate_X(distance);
        }
        public override void Translate_Y(float distance)
        {
            base.Translate_Y(distance);
            foreach (SceneObject scene_object in Scene_Objects) scene_object.Translate_Y(distance);
        }
        public override void Translate_Z(float distance)
        {
            base.Translate_Z(distance);
            foreach (SceneObject scene_object in Scene_Objects) scene_object.Translate_Z(distance);
        }
        public override void Translate(Vector3D displacement)
        {
            base.Translate(displacement);
            foreach (SceneObject scene_object in Scene_Objects) scene_object.Translate(displacement);
        }

        #endregion
    }
}
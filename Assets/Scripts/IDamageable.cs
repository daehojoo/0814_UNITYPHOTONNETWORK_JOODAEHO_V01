using UnityEngine;

public interface IDamageable
{                   //�������̽� ��ü�� ������ Ŀ�ø��̶� ��   //���״�� �����ϰ� �˻���
                    //�� �޼��带 �޴� Ŭ������ � Ŭ��������
                    //�˻縦 ���� �ʴ´�.
    void OnDamage(float damage,Vector3 hitPoint, Vector3 hitNormal);
}

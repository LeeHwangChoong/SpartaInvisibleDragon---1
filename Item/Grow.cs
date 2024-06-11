using System.Collections;
using UnityEditor;
using UnityEngine;

public class Grow : MonoBehaviour, IInteractable
{
    public GameObject grow1;
    public GameObject grow2;
    private float growthTime = 3f;


    public Vector3 cropPositionOffset = Vector3.zero; // ���۹� ��ġ ������

    private int currentStage = 0;
    private bool canHarvest = false;
    private GameObject currentCrop;

    public GameObject Prefab;


    private Vector3 maxScale1 = new Vector3(0.7f, 0.8f, 0.7f); // ���� �ִ� ũ��
    private Vector3 minScale1 = new Vector3(0.2f, 0.3f, 0.2f); // ���� �ʱ� ũ��

    private Vector3 maxScale2 = new Vector3(0.5f, 0.7f, 0.5f); // ��Ȯ�� �ִ� ũ��
    private Vector3 minScale2 = new Vector3(0.5f, 0.5f, 0.5f); // ��Ȯ�� �ʱ� ũ��

    private bool ckgrow = false;
    private float growthDuration;
    private void Start()
    {
        //currentCrop = Instantiate(grow0, transform.position + cropPositionOffset, Quaternion.identity, transform);
        growthDuration = growthTime * 2; // ��ü ���� �Ⱓ (2�ܰ� ���ĺ��� ��Ȯ ����)
        StartCoroutine(GrowCrop());
    }

    IEnumerator GrowCrop()
    {
        while (currentStage < 2)
        {
            yield return new WaitForSeconds(growthTime);
            currentStage++;
            UpdateCropModel();
        }
        canHarvest = true;
    }
    void UpdateCropModel()
    {
        if (currentCrop != null)
        {
            Destroy(currentCrop);
        }

        switch (currentStage)
        {
            case 1:
                currentCrop = Instantiate(grow1, transform.position + cropPositionOffset, Quaternion.identity, transform);
                //StartCoroutine(ScaleOverTime(transform, minScale1, maxScale1, growthDuration));
                StartCoroutine(ScaleOverTime(currentCrop.transform, minScale1, maxScale1, growthDuration));
                break;
            case 2:
                currentCrop = Instantiate(grow2, transform.position + cropPositionOffset, Quaternion.identity, transform);
                //StartCoroutine(ScaleOverTime(transform, minScale2, maxScale2, growthDuration));
                StartCoroutine(ScaleOverTime(currentCrop.transform, minScale2, maxScale2, growthDuration));
                break;
        }
    }
    IEnumerator ScaleOverTime(Transform objTransform, Vector3 startScale, Vector3 endScale, float duration)
    {
        float currentTime = 0.0f;

        while (currentTime <= duration)
        {
            if (objTransform == null) yield break; // ������Ʈ�� �ı��Ǿ����� Ȯ��

            objTransform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        if (objTransform != null) // �������� Ȯ��
        {
            objTransform.localScale = endScale;
            ckgrow = true;
        }
    }
    public void HarvestCrop()
    {
        if (ckgrow)
        {
            Destroy(gameObject);
            DropItem();
        }
    }

    void DropItem()
    {
        if (currentCrop != null)
        {
            //Debug.Log(currentCrop);
            // ������ ����
            string prefabName = Prefab.name; // ������ ������ �̸� ����
            Vector3 spawnPosition = transform.position + new Vector3(0, 2.0f, 0);
            ItemDataManager.Instance.CreateItemObject(prefabName, 1, spawnPosition);
        }
    }

    public string GetInteractPrompt()
    {
        string str = "���� ��....";
        if (ckgrow)
        {
            str = "[E] Ű�� ���� ��Ȯ";
            return str;
        }
        return str;
    }

    public void OnInteract()
    {
        if (ckgrow)
        {
            HarvestCrop();
            DropItem();
        }
        
    }

}
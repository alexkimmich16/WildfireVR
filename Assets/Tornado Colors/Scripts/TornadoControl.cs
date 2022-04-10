using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TornadoControl : MonoBehaviour
{
    [Header("Colors ---------------------------")]
    [ColorUsage(true, true)]
    public Color wcolor;
    [ColorUsage(true, true)]
    public Color bcolor;
    [ColorUsage(true, true)]
    public Color color1;
    [ColorUsage(true, true)]
    public Color color2;

    [Header("Dont Change if Not Necessary ---------------------------")]
    public Transform[] black_parts;
    public Transform[] white_parts;
    public Transform[] color1_parts;
    public Transform[] color2_parts;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < black_parts.Length; i++) {
            black_parts[i].GetComponent<Renderer>().material.SetColor("_Color1", bcolor);
        }
        for (int i = 0; i < white_parts.Length; i++)
        {
            white_parts[i].GetComponent<Renderer>().material.SetColor("_Color1", wcolor);
        }
        for (int i = 0; i < color1_parts.Length; i++)
        {
            color1_parts[i].GetComponent<Renderer>().material.SetColor("_Color1", color1);
        }
        for (int i = 0; i < color2_parts.Length; i++)
        {
            color2_parts[i].GetComponent<Renderer>().material.SetColor("_Color1", color2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform tornado = transform;


        if (Input.GetMouseButtonDown(0)) {
            tornado.GetComponent<TornadoControl>().Tournado_1_PlayTrails();
            //tornado.GetComponent<TornadoControl>().Tournado_2_ShowChargeBall();
            //tornado.GetComponent<TornadoControl>().Tournado_3_ShowTournado();
            //tornado.GetComponent<TornadoControl>().Tournado_4_Finish();
        }


    }

    public void Tournado_1_PlayTrails() {
        transform.Find("Trails").gameObject.SetActive(true);
    }

    public void Tournado_2_ShowChargeBall()
    {
        transform.Find("Charge Ball").gameObject.SetActive(true);
    }

    public IEnumerator Tournado_3_ShowTornado()
    {
        Transform part = transform.Find("Trails");
        for (int i = 0; i < part.childCount; i++)
        {
            EmissionModule e = part.GetChild(i).GetComponent<ParticleSystem>().emission;
            e.enabled = false;
        }

        Transform ball = transform.Find("Charge Ball");
        
        float timer = 0;
        float maxTime = 0.7f;

        transform.Find("Rim").gameObject.SetActive(true);

        while (timer <= maxTime) {
            float s = timer/maxTime;

            ball.Find("Circle").localScale = new Vector3(1-s, 1, 1-s);
            ball.Find("Circle").Rotate(0, 5, 0);

            timer += Time.deltaTime;

            yield return null;
        }
        
        transform.Find("Start Explos").gameObject.SetActive(true);
	if(Camera.main.GetComponent<CameraShake>() != null){
            Camera.main.GetComponent<CameraShake>().Shake(0.25f, 1.5f);
	}
        transform.Find("Tornado").gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        transform.Find("Charge Ball").gameObject.SetActive(false);
    }

    public IEnumerator Tournado_4_Finish()
    {
        transform.Find("Finish Explos").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        transform.Find("Tornado").gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}

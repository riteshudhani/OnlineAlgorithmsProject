using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OrbGeneration : MonoBehaviour
{
    string filePath;
    int arrayLength;
    Vector3[] a;
    Vector3[] b;
    Vector3[] c;
    Vector4[] d;
    Vector4[] e;
    Vector4[] f;
    Vector3[] g;
    Vector3[] h;
    Vector3[] rrToeXYZ;
    Vector4[] m;
    Vector4[] k;
    Vector4[] l;
    float[] timeStamps;

    GameObject orb;
    // Start is called before the first frame update
    void Start()
    {
        orb = GameObject.Find("SphereYellow");
        // filePath = "Assets/groundtruthdata1.txt";
         filePath = "Assets/groundtruthdata11.txt";
        // filePath = "Assets/groundtruthdata14.txt";

        fileload();
    }

    void fileload()
    {
        arrayLength = File.ReadAllLines(filePath).Length;
        a = new Vector3[arrayLength - 1];
        b = new Vector3[arrayLength - 1];
        c = new Vector3[arrayLength - 1];
        d = new Vector4[arrayLength - 1];
        e = new Vector4[arrayLength - 1];
        f = new Vector4[arrayLength - 1];

        g = new Vector3[arrayLength - 1];
        h = new Vector3[arrayLength - 1];
        rrToeXYZ = new Vector3[arrayLength - 1];
        k = new Vector4[arrayLength - 1];
        l = new Vector4[arrayLength - 1];
        m = new Vector4[arrayLength - 1];
        timeStamps = new float[arrayLength - 1];


        StreamReader reader = new StreamReader(filePath);
        string itemStrings = reader.ReadLine();
        //call ReadLine twice to skip text headers
        itemStrings = reader.ReadLine();
        char[] delimiter = { ' ' };

        int i = 0;
        while (itemStrings != null)
        {
            string[] fields = itemStrings.Split(delimiter);

            timeStamps[i] = float.Parse(fields[0]);

            a[i] = new Vector3(float.Parse(fields[1]), float.Parse(fields[2]), float.Parse(fields[3]));
            b[i] = new Vector3(float.Parse(fields[4]), float.Parse(fields[5]), float.Parse(fields[6]));
            rrToeXYZ[i] = new Vector3(float.Parse(fields[7]), float.Parse(fields[8])+0.1f, float.Parse(fields[9]));

            c[i] = new Vector3(float.Parse(fields[10]), float.Parse(fields[11]), float.Parse(fields[12]));
            d[i] = new Vector3(float.Parse(fields[13]), float.Parse(fields[14]), float.Parse(fields[15]));
            e[i] = new Vector3(float.Parse(fields[16]), float.Parse(fields[17]), float.Parse(fields[18]));

            //x,y,z,w
            f[i] = new Vector4(float.Parse(fields[38]), float.Parse(fields[39]), float.Parse(fields[40]), float.Parse(fields[37]));
            g[i] = new Vector4(float.Parse(fields[42]), float.Parse(fields[43]), float.Parse(fields[44]), float.Parse(fields[41]));
            h[i] = new Vector4(float.Parse(fields[46]), float.Parse(fields[47]), float.Parse(fields[48]), float.Parse(fields[45]));

            m[i] = new Vector4(float.Parse(fields[50]), float.Parse(fields[51]), float.Parse(fields[52]), float.Parse(fields[49]));
            k[i] = new Vector4(float.Parse(fields[54]), float.Parse(fields[55]), float.Parse(fields[56]), float.Parse(fields[53]));
            l[i] = new Vector4(float.Parse(fields[58]), float.Parse(fields[59]), float.Parse(fields[60]), float.Parse(fields[57]));


            itemStrings = reader.ReadLine();

            i++;
        }
      /*    Instantiate(orb, rrToeXYZ[0], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[1], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[2], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[3], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[4], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[5], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[6], Quaternion.identity);
        Instantiate(orb, rrToeXYZ[7], Quaternion.identity);*/
         for (int j = 0; j <= arrayLength-2; j++)
         {
            Debug.LogError("Ritesh:" + j);
          Instantiate(orb, rrToeXYZ[j], Quaternion.identity);
        }
        Debug.LogError("Ritesh:" + arrayLength);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

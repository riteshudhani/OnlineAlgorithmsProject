/* Filename:    OrbsGeneration.cs
 * Author:      Ritesh Udhani
 * Date:        16th Dec 
 * Purpose:     Generated for project of Online Algorithms course
 * License:     MIT license
 * Details:     This file is a part of a larger project on motion tracking, visualization and comparison system. 
 *              The file contains 2 algorithms used to place orbs for the reference motion pathway with 3 different sizes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrbsGeneration : MonoBehaviour
{
    string filePath;
    string directory;
    string[] anglesToWrite = new string[7];
    string[] groundTruthPositionsToWrite = new string[65];
    string[] groundTruthPositionsPrev1 = new string[65];
    string[] groundTruthPositionsPrev2 = new string[65];
    string[] groundTruthPositionsPrev3 = new string[65];
    string[] groundTruthPositionsPrev4 = new string[65];
    string[] groundTruthPositionsPrev5 = new string[65];
    string[] groundTruthPositionsPrev6 = new string[65];
    string[] groundTruthPositionsTemp = new string[65];
    int timePrev;
    double[] dgroundTruthPositionsPrev = new double[65];

    public static string whichLeg;

    double[] dpreviousPos1 = new double[3];
    double[] dpreviousPos2 = new double[3];
    double[] dpreviousPos3 = new double[3];
    double[] dcurrentOrb4 = new double[3];
    double[] dcurrentOrb5 = new double[3];
    double[] dcurrentOrb6 = new double[3];
    double[] dcurrentPos = new double[3];
    double[] dvectorStart4 = new double[3];
    double[] dvectorStart5 = new double[3];
    double[] dvectorStart6 = new double[3];
    double[] dvectorEnd4 = new double[3];
    double[] dvectorEnd5 = new double[3];
    double[] dvectorEnd6 = new double[3];

    double dradius1;
    double dradius2;
    double dradius3;
    double dratio4;
    double dratio5;
    double dratio6;

    bool acceptPoint1;
    bool acceptPoint2;
    bool acceptPoint3;
    bool acceptPoint4;
    bool acceptPoint5;
    bool acceptPoint6;

    bool[] writeFile = new bool[2];

    int fitableOrb1;
    int fitableOrb2;
    int fitableOrb3;
    int fitableOrb4;
    int fitableOrb5;
    int fitableOrb6;

    public static bool firstWrite1 = true;
    public static bool firstWrite4 = true;
    public static bool firstWrite5 = true;
    public static bool firstWrite6 = true;

    bool alg1 = true; //Orb generation through Threshold Based Orb Generation algorithm
    bool alg2 = true; //Orb generation through Vector Based Orb Generation algorithm

    double d4, l24, l14; //d = distance between vector l2 and l1
    double d5, l25, l15;
    double d6, l26, l16;

    void Start() //Called during the start of the app
    {
        whichLeg = StartApplicationButton.whichLeg;

        //3 radii of orbs which will be used for reference pathway generation. Input value of UI element DifficultySlider's max and min values used to calculated the radii
        dradius1 = DifficultySlider.slider.minValue/2.0;
        dradius2 = (DifficultySlider.slider.minValue + DifficultySlider.slider.maxValue) / 4.0;
        dradius3 = DifficultySlider.slider.maxValue/2.0;
    }

    //Update is invoked in everyanimation frame
    void Update()
    {
        //once recording button for reference file has been pressed do the following
        if (writeFile[1] == true)
        {
            //Store data other than position in groundTruthPositionsToWrite (Not relevant to orb placement logic)
            groundTruthPositionsToWrite[0] = (stopWatch[1].ElapsedMilliseconds).ToString();
            groundTruthPositionsToWrite[1] = Object2Mover.knee.transform.position.x.ToString();
            groundTruthPositionsToWrite[2] = Object2Mover.knee.transform.position.y.ToString();
            groundTruthPositionsToWrite[3] = Object2Mover.knee.transform.position.z.ToString();
            groundTruthPositionsToWrite[4] = Object3Mover.ankle.transform.position.x.ToString();
            groundTruthPositionsToWrite[5] = Object3Mover.ankle.transform.position.y.ToString();
            groundTruthPositionsToWrite[6] = Object3Mover.ankle.transform.position.z.ToString();

            if (whichLeg == "Right") //specifies the leg, right or left which is being tracked and store the position of toes
            {
                dcurrentPos[0] = RightToeFinder.rightToes.transform.position.x;
                dcurrentPos[1] = RightToeFinder.rightToes.transform.position.y;
                dcurrentPos[2] = RightToeFinder.rightToes.transform.position.z;
            }
            else
            {
                dcurrentPos[0] = LeftToeFinder.leftToes.transform.position.x;
                dcurrentPos[1] = LeftToeFinder.leftToes.transform.position.y;
                dcurrentPos[2] = LeftToeFinder.leftToes.transform.position.z;
            }

            //Coordinates of toes of right/left leg store in groundTruthPositionsToWrite (Relevant to orb placement logic)
            groundTruthPositionsToWrite[7] = dcurrentPos[0].ToString();
            groundTruthPositionsToWrite[8] = dcurrentPos[1].ToString();
            groundTruthPositionsToWrite[9] = dcurrentPos[2].ToString();

            //Logic of Threshold Based Orb Generation algorithm
            if (alg1)
            {
                if (firstWrite1)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        dpreviousPos1[i] = dcurrentPos[i];
                        dpreviousPos2[i] = dcurrentPos[i];
                        dpreviousPos3[i] = dcurrentPos[i];
                    }
                }

                double gap1 = Math.Sqrt(Math.Pow(dpreviousPos1[0] - dcurrentPos[0], 2.0) + Math.Pow(dpreviousPos1[1] - dcurrentPos[1], 2.0) + Math.Pow(dpreviousPos1[2] - dcurrentPos[2], 2.0));
                double gap2 = Math.Sqrt(Math.Pow(dpreviousPos2[0] - dcurrentPos[0], 2.0) + Math.Pow(dpreviousPos2[1] - dcurrentPos[1], 2.0) + Math.Pow(dpreviousPos2[2] - dcurrentPos[2], 2.0));
                double gap3 = Math.Sqrt(Math.Pow(dpreviousPos3[0] - dcurrentPos[0], 2.0) + Math.Pow(dpreviousPos3[1] - dcurrentPos[1], 2.0) + Math.Pow(dpreviousPos3[2] - dcurrentPos[2], 2.0));

                if ((gap1 > (2.0 * dradius1)) || firstWrite1)
                {
                    acceptPoint1 = true;

                    if (!firstWrite1)
                    {
                        fitableOrb1 = (int)Math.Floor((gap1 - (2.0 * dradius1)) / (2.0 * dradius1));
                    }

                    dpreviousPos1[0] = dcurrentPos[0];
                    dpreviousPos1[1] = dcurrentPos[1];
                    dpreviousPos1[2] = dcurrentPos[2];
                }
                else
                {
                    acceptPoint1 = false;
                }
                if ((gap2 > (2.0 * dradius2)) || firstWrite1)
                {
                    acceptPoint2 = true;
                    if (!firstWrite1)
                    {
                        fitableOrb2 = (int)Math.Floor((gap2 - (2.0 * dradius2)) / (2.0 * dradius2));
                    }

                    dpreviousPos2[0] = dcurrentPos[0];
                    dpreviousPos2[1] = dcurrentPos[1];
                    dpreviousPos2[2] = dcurrentPos[2];
                }
                else
                {
                    acceptPoint2 = false;
                }
                if ((gap3 > (2.0 * dradius3)) || firstWrite1)
                {
                    acceptPoint3 = true;
                    if (!firstWrite1)
                    {
                        fitableOrb3 = (int)Math.Floor((gap3 - (2.0 * dradius3)) / (2.0 * dradius3));
                    }

                    dpreviousPos3[0] = dcurrentPos[0];
                    dpreviousPos3[1] = dcurrentPos[1];
                    dpreviousPos3[2] = dcurrentPos[2];
                }
                else
                {
                    acceptPoint3 = false;
                }

                firstWrite1 = false;
            }

            //Vector Based Orb Generation algorithm
            if (alg2)
            {
                if (firstWrite4 && firstWrite5 && firstWrite6)
                {
                    fitableOrb4 = 1;
                    fitableOrb5 = 1;
                    fitableOrb6 = 1;

                    for (int i = 0; i <= 2; i++)
                    {
                        dcurrentOrb4[i] = dvectorStart4[i] = dvectorEnd4[i] = dcurrentPos[i];
                        dcurrentOrb5[i] = dvectorStart5[i] = dvectorEnd5[i] = dcurrentPos[i];
                        dcurrentOrb6[i] = dvectorStart6[i] = dvectorEnd6[i] = dcurrentPos[i];
                    }
                    acceptPoint4 = true;
                    acceptPoint5 = true;
                    acceptPoint6 = true;
                }
                else
                {
                    acceptPoint4 = false;
                    acceptPoint5 = false;
                    acceptPoint6 = false;

                    for (int i = 0; i <= 2; i++)
                    {
                        dvectorEnd4[i] = dcurrentPos[i];
                        dvectorEnd5[i] = dcurrentPos[i];
                        dvectorEnd6[i] = dcurrentPos[i];
                    }

                    d4 = Math.Sqrt(Math.Pow((dvectorEnd4[0] - dvectorStart4[0]), 2) + Math.Pow((dvectorEnd4[1] - dvectorStart4[1]), 2) + Math.Pow((dvectorEnd4[2] - dvectorStart4[2]), 2));
                    l24 = Math.Sqrt(Math.Pow((dvectorEnd4[0] - dcurrentOrb4[0]), 2) + Math.Pow((dvectorEnd4[1] - dcurrentOrb4[1]), 2) + Math.Pow((dvectorEnd4[2] - dcurrentOrb4[2]), 2));
                    l14 = Math.Sqrt(Math.Pow((dvectorStart4[0] - dcurrentOrb4[0]), 2) + Math.Pow((dvectorStart4[1] - dcurrentOrb4[1]), 2) + Math.Pow((dvectorStart4[2] - dcurrentOrb4[2]), 2));

                    if (l24 >= (2.0 * dradius1))
                    {
                        if (l24 - l14 == d4 && l24 != l14)
                        {
                            dratio4 = ((2 * dradius1) - l14) / (l24 - l14);
                         }
                        else
                        {
                            if (d4 != 0.0)
                            {
                                double y = ((Math.Pow(l24, 2) - Math.Pow(l14, 2) - Math.Pow(d4, 2)) / (2 * d4));
                                double x = (Math.Sqrt(Math.Pow(l14, 2) - Math.Pow(y, 2)));
                                dratio4 = ((Math.Sqrt(Math.Pow(2 * dradius1, 2) - Math.Pow(x, 2)) - y) / (d4));
                            }
                         }
                         acceptPoint4 = true;
                         fitableOrb4 = (int)Math.Floor((l24) / (2.0 * dradius1));
                    }

                    firstWrite4 = false;

                    d5 = Math.Sqrt(Math.Pow((dvectorEnd5[0] - dvectorStart5[0]), 2) + Math.Pow((dvectorEnd5[1] - dvectorStart5[1]), 2) + Math.Pow((dvectorEnd5[2] - dvectorStart5[2]), 2));
                    l25 = Math.Sqrt(Math.Pow((dvectorEnd5[0] - dcurrentOrb5[0]), 2) + Math.Pow((dvectorEnd5[1] - dcurrentOrb5[1]), 2) + Math.Pow((dvectorEnd5[2] - dcurrentOrb5[2]), 2));
                    l15 = Math.Sqrt(Math.Pow((dvectorStart5[0] - dcurrentOrb5[0]), 2) + Math.Pow((dvectorStart5[1] - dcurrentOrb5[1]), 2) + Math.Pow((dvectorStart5[2] - dcurrentOrb5[2]), 2));

                    if (l25 >= (2.0 * dradius2))
                    {
                        if (l25 - l15 == d5 && l25 != l15)
                        {
                            dratio5 = ((2 * dradius2) - l15) / (l25 - l15);
                        }
                        else
                        {
                            if (d5 != 0.0)
                            {
                                double y = ((Math.Pow(l25, 2) - Math.Pow(l15, 2) - Math.Pow(d5, 2)) / (2 * d5));
                                double x = (Math.Sqrt(Math.Pow(l15, 2) - Math.Pow(y, 2)));
                                dratio5 = ((Math.Sqrt(Math.Pow(2 * dradius2, 2) - Math.Pow(x, 2)) - y) / (d5));
                            }
                        }
                        acceptPoint5 = true;

                        fitableOrb5 = (int)Math.Floor((l25) / (2.0 * dradius2));
                    }

                    firstWrite5 = false;

                    d6 = Math.Sqrt(Math.Pow((dvectorEnd6[0] - dvectorStart6[0]), 2) + Math.Pow((dvectorEnd6[1] - dvectorStart6[1]), 2) + Math.Pow((dvectorEnd6[2] - dvectorStart6[2]), 2));
                    l26 = Math.Sqrt(Math.Pow((dvectorEnd6[0] - dcurrentOrb6[0]), 2) + Math.Pow((dvectorEnd6[1] - dcurrentOrb6[1]), 2) + Math.Pow((dvectorEnd6[2] - dcurrentOrb6[2]), 2));
                    l16 = Math.Sqrt(Math.Pow((dvectorStart6[0] - dcurrentOrb6[0]), 2) + Math.Pow((dvectorStart6[1] - dcurrentOrb6[1]), 2) + Math.Pow((dvectorStart6[2] - dcurrentOrb6[2]), 2));

                    if (l26 >= (2.0 * dradius3))
                    {
                        if (l26 - l16 == d6 && l26 != l16)
                        {
                            dratio6 = ((2 * dradius3) - l16) / (l26 - l16);
                        }
                        else
                        {
                            if (d6 != 0.0)
                            {
                                double y = ((Math.Pow(l26, 2) - Math.Pow(l16, 2) - Math.Pow(d6, 2)) / (2 * d6));
                                double x = (Math.Sqrt(Math.Pow(l16, 2) - Math.Pow(y, 2)));
                                dratio6 = ((Math.Sqrt(Math.Pow(2 * dradius3, 2) - Math.Pow(x, 2)) - y) / (d6));
                            }
                        }
                        acceptPoint6 = true;

                        fitableOrb6 = (int)Math.Floor((l26) / (2.0 * dradius3));
                    }
                    firstWrite6 = false;
                }
            }

            //Writing other non relevant data for orb placement to groundTruthPositionsToWrite
            groundTruthPositionsToWrite[10] = Object5Mover.knee2.transform.position.x.ToString();
            groundTruthPositionsToWrite[11] = Object5Mover.knee2.transform.position.y.ToString();
            groundTruthPositionsToWrite[12] = Object5Mover.knee2.transform.position.z.ToString();
            groundTruthPositionsToWrite[13] = Object6Mover.ankle2.transform.position.x.ToString();
            groundTruthPositionsToWrite[14] = Object6Mover.ankle2.transform.position.y.ToString();
            groundTruthPositionsToWrite[15] = Object6Mover.ankle2.transform.position.z.ToString();

            //Based upon left or right leg, the position of the toes of other leg also saved but not used in single leg tracking
            if (whichLeg == "Right")
            {
                groundTruthPositionsToWrite[16] = LeftToeFinder.leftToes.transform.position.x.ToString();
                groundTruthPositionsToWrite[17] = LeftToeFinder.leftToes.transform.position.y.ToString();
                groundTruthPositionsToWrite[18] = LeftToeFinder.leftToes.transform.position.z.ToString();
            }
            else
            {
                groundTruthPositionsToWrite[16] = RightToeFinder.rightToes.transform.position.x.ToString();
                groundTruthPositionsToWrite[17] = RightToeFinder.rightToes.transform.position.y.ToString();
                groundTruthPositionsToWrite[18] = RightToeFinder.rightToes.transform.position.z.ToString();
            }

            //Writing other non relevant data for orb placement to groundTruthPositionsToWrite
            groundTruthPositionsToWrite[19] = rThigh_ox.ToString();
            groundTruthPositionsToWrite[20] = rThigh_oy.ToString();
            groundTruthPositionsToWrite[21] = rThigh_oz.ToString();
            groundTruthPositionsToWrite[22] = rCalf_ox.ToString();
            groundTruthPositionsToWrite[23] = rCalf_oy.ToString();
            groundTruthPositionsToWrite[24] = rCalf_oz.ToString();
            groundTruthPositionsToWrite[25] = rFoot_ox.ToString();
            groundTruthPositionsToWrite[26] = rFoot_oy.ToString();
            groundTruthPositionsToWrite[27] = rFoot_oz.ToString();
            groundTruthPositionsToWrite[28] = lThigh_ox.ToString();
            groundTruthPositionsToWrite[29] = lThigh_oy.ToString();
            groundTruthPositionsToWrite[30] = lThigh_oz.ToString();
            groundTruthPositionsToWrite[31] = lCalf_ox.ToString();
            groundTruthPositionsToWrite[32] = lCalf_oy.ToString();
            groundTruthPositionsToWrite[33] = lCalf_oz.ToString();
            groundTruthPositionsToWrite[34] = lFoot_ox.ToString();
            groundTruthPositionsToWrite[35] = lFoot_oy.ToString();
            groundTruthPositionsToWrite[36] = lFoot_oz.ToString();
            groundTruthPositionsToWrite[37] = transformQuaternion.w.ToString();
            groundTruthPositionsToWrite[38] = transformQuaternion.x.ToString();
            groundTruthPositionsToWrite[39] = transformQuaternion.y.ToString();
            groundTruthPositionsToWrite[40] = transformQuaternion.z.ToString();
            groundTruthPositionsToWrite[41] = Object2Mover.transformQuaternion2.w.ToString();
            groundTruthPositionsToWrite[42] = Object2Mover.transformQuaternion2.x.ToString();
            groundTruthPositionsToWrite[43] = Object2Mover.transformQuaternion2.y.ToString();
            groundTruthPositionsToWrite[44] = Object2Mover.transformQuaternion2.z.ToString();
            groundTruthPositionsToWrite[45] = Object3Mover.transformQuaternion3.w.ToString();
            groundTruthPositionsToWrite[46] = Object3Mover.transformQuaternion3.x.ToString();
            groundTruthPositionsToWrite[47] = Object3Mover.transformQuaternion3.y.ToString();
            groundTruthPositionsToWrite[48] = Object3Mover.transformQuaternion3.z.ToString();
            groundTruthPositionsToWrite[49] = Object4Mover.transformQuaternion4.w.ToString();
            groundTruthPositionsToWrite[50] = Object4Mover.transformQuaternion4.x.ToString();
            groundTruthPositionsToWrite[51] = Object4Mover.transformQuaternion4.y.ToString();
            groundTruthPositionsToWrite[52] = Object4Mover.transformQuaternion4.z.ToString();
            groundTruthPositionsToWrite[53] = Object5Mover.transformQuaternion5.w.ToString();
            groundTruthPositionsToWrite[54] = Object5Mover.transformQuaternion5.x.ToString();
            groundTruthPositionsToWrite[55] = Object5Mover.transformQuaternion5.y.ToString();
            groundTruthPositionsToWrite[56] = Object5Mover.transformQuaternion5.z.ToString();
            groundTruthPositionsToWrite[57] = Object6Mover.transformQuaternion6.w.ToString();
            groundTruthPositionsToWrite[58] = Object6Mover.transformQuaternion6.x.ToString();
            groundTruthPositionsToWrite[59] = Object6Mover.transformQuaternion6.y.ToString();
            groundTruthPositionsToWrite[60] = Object6Mover.transformQuaternion6.z.ToString();
            groundTruthPositionsToWrite[61] = Object7Mover.transformQuaternion7.w.ToString();
            groundTruthPositionsToWrite[62] = Object7Mover.transformQuaternion7.x.ToString();
            groundTruthPositionsToWrite[63] = Object7Mover.transformQuaternion7.y.ToString();
            groundTruthPositionsToWrite[64] = Object7Mover.transformQuaternion7.z.ToString();

            //Logic of Threshold Based Orb Generation algorithm continues
            if (alg1)
            {
                //File writing for 3 different orb sizes with this algorithm
                using (var writer1 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "1.txt", append: true))
                using (var writer2 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "2.txt", append: true))
                using (var writer3 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "3.txt", append: true))
                {
                    string tempString1 = null;
                    string tempString2 = null;
                    string tempString3 = null;

                    if (acceptPoint1)
                    {
                        if (fitableOrb1 > 0)
                        {
                            for (int j = 1; j <= fitableOrb1; j++)
                            {
                                tempString1 = null;
                                for (int i = 0; i < 65; i++)
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = ((Convert.ToInt32(groundTruthPositionsPrev1[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev1[i])) * j) / (fitableOrb1 + 1))).ToString();
                                    }
                                    else
                                    {
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev1[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev1[i])) * j) / (fitableOrb1 + 1)).ToString();
                                    }
                                    tempString1 = tempString1 + groundTruthPositionsTemp[i].ToString() + " ";
                                }
                                writer1.WriteLine(tempString1);
                            }

                            tempString1 = null;
                        }
                        for (int i = 0; i < 65; i++)
                        {
                            tempString1 = tempString1 + groundTruthPositionsToWrite[i].ToString() + " ";
                            groundTruthPositionsPrev1[i] = groundTruthPositionsToWrite[i];
                        }
                        writer1.WriteLine(tempString1);
                        Debug.LogError("Ritesh:" + tempString1);
                    }

                    if (acceptPoint2)
                    {
                        if (fitableOrb2 > 0)
                        {
                            for (int j = 1; j <= fitableOrb2; j++)
                            {
                                tempString2 = null;
                                for (int i = 0; i < 65; i++)
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = (Convert.ToInt32(groundTruthPositionsPrev2[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev2[i])) * j) / (fitableOrb2 + 1)).ToString();
                                    }
                                    else
                                    {
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev2[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev2[i])) * j) / (fitableOrb2 + 1)).ToString();
                                    }
                                    tempString2 = tempString2 + groundTruthPositionsTemp[i].ToString() + " ";
                                }
                                writer2.WriteLine(tempString2);
                            }

                            tempString2 = null;
                        }
                        for (int i = 0; i < 65; i++)
                        {
                            tempString2 = tempString2 + groundTruthPositionsToWrite[i].ToString() + " ";
                            groundTruthPositionsPrev2[i] = groundTruthPositionsToWrite[i];
                        }
                        writer2.WriteLine(tempString2);
                    }

                    if (acceptPoint3)
                    {
                        if (fitableOrb3 > 0)
                        {
                            for (int j = 1; j <= fitableOrb3; j++)
                            {
                                tempString3 = null;
                                for (int i = 0; i < 65; i++)
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = (Convert.ToInt32(groundTruthPositionsPrev3[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev3[i])) * j) / (fitableOrb3 + 1)).ToString();
                                    }
                                    else
                                    {
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev3[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev3[i])) * j) / (fitableOrb3 + 1)).ToString();
                                    }
                                    tempString3 = tempString3 + groundTruthPositionsTemp[i].ToString() + " ";
                                }
                                writer3.WriteLine(tempString3);
                            }

                            tempString3 = null;
                        }

                        for (int i = 0; i < 65; i++)
                        {
                            tempString3 = tempString3 + groundTruthPositionsToWrite[i].ToString() + " ";
                            groundTruthPositionsPrev3[i] = groundTruthPositionsToWrite[i];
                        }
                        writer3.WriteLine(tempString3);
                    }

                    writer1.Close();
                    writer2.Close();
                    writer3.Close();
                }
            }

            //Vector Based Orb Generation algorithm continues
            if (alg2)
            {
                //File writing for 3 different orb sizes with vector based algorithm
                using (var writer4 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "4.txt", append: true))
                using (var writer5 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "5.txt", append: true))
                using (var writer6 = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + "6.txt", append: true))
                {

                    string tempString4 = null;
                    string tempString5 = null;
                    string tempString6 = null;
                                        
                    if (acceptPoint4)
                    {
                        for (int j = 1; j <= fitableOrb4; j++)
                        {
                            tempString4 = null;
                            for (int i = 0; i < 65; i++)
                            {
                                if (firstWrite4)
                                {
                                    groundTruthPositionsTemp[i] = groundTruthPositionsPrev4[i] = groundTruthPositionsToWrite[i];
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = Math.Round((Convert.ToInt32(groundTruthPositionsPrev4[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev4[i])) * dratio4))).ToString();
                                    }
                                    else
                                    {
                                        if (i == 7 || i == 8 || i == 9)
                                        {
                                            dcurrentOrb4[i - 7] = Convert.ToDouble(groundTruthPositionsPrev4[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev4[i])) * dratio4);
                                        }
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev4[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev4[i])) * dratio4)).ToString();
                                    }
                                }
                                tempString4 = tempString4 + groundTruthPositionsTemp[i].ToString() + " ";
                            }
                            if (firstWrite4)
                            {
                                firstWrite4 = false;
                            }

                            double y = ((Math.Pow(l24, 2) - Math.Pow(l14, 2) - Math.Pow(d4, 2)) / (2 * d4));
                            double x = (Math.Sqrt(Math.Pow(l14, 2) - Math.Pow(y, 2)));
                            dratio4 = ((Math.Sqrt(Math.Pow(2 * (j+1)*dradius1, 2) - Math.Pow(x, 2)) - y) / (d4));

                            writer4.WriteLine(tempString4);
                        }
                    }
                    fitableOrb4 = 0;
                    writer4.Close();

                    for (int i = 0; i <= 2; i++)
                    {
                        dvectorStart4[i] = dvectorEnd4[i];
                    }

                    if (acceptPoint5)
                    {
                        for (int j = 1; j <= fitableOrb5; j++)
                        {
                            tempString5 = null;
                            for (int i = 0; i < 65; i++)
                            {
                                if (firstWrite5)
                                {
                                    groundTruthPositionsTemp[i] = groundTruthPositionsPrev5[i] = groundTruthPositionsToWrite[i];
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = Math.Round((Convert.ToInt32(groundTruthPositionsPrev5[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev5[i])) * dratio5))).ToString();
                                    }
                                    else
                                    {
                                        if (i == 7 || i == 8 || i == 9)
                                        {
                                            dcurrentOrb5[i - 7] = Convert.ToDouble(groundTruthPositionsPrev5[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev5[i])) * dratio5);
                                        }
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev5[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev5[i])) * dratio5)).ToString();
                                    }
                                }
                                tempString5 = tempString5 + groundTruthPositionsTemp[i].ToString() + " ";
                            }
                            if (firstWrite5)
                            {
                                firstWrite5 = false;
                            }

                            double y = ((Math.Pow(l25, 2) - Math.Pow(l15, 2) - Math.Pow(d5, 2)) / (2 * d5));
                            double x = (Math.Sqrt(Math.Pow(l15, 2) - Math.Pow(y, 2)));
                            dratio5 = ((Math.Sqrt(Math.Pow(2 * (j + 1) * dradius2, 2) - Math.Pow(x, 2)) - y) / (d5));

                            writer5.WriteLine(tempString5);
                        }
                    }
                    fitableOrb5 = 0;
                    writer5.Close();

                    for (int i = 0; i <= 2; i++)
                    {
                        dvectorStart5[i] = dvectorEnd5[i];
                    }

                    if (acceptPoint6)
                    {
                        for (int j = 1; j <= fitableOrb6; j++)
                        {
                            tempString6 = null;
                            for (int i = 0; i < 65; i++)
                            {
                                if (firstWrite6)
                                {
                                    groundTruthPositionsTemp[i] = groundTruthPositionsPrev6[i] = groundTruthPositionsToWrite[i];
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        groundTruthPositionsTemp[i] = Math.Round((Convert.ToInt32(groundTruthPositionsPrev6[i]) + ((Convert.ToInt32(groundTruthPositionsToWrite[i]) - Convert.ToInt32(groundTruthPositionsPrev6[i])) * dratio6))).ToString();
                                    }
                                    else
                                    {
                                        if (i == 7 || i == 8 || i == 9)
                                        {
                                            dcurrentOrb6[i - 7] = Convert.ToDouble(groundTruthPositionsPrev6[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev6[i])) * dratio6);
                                        }
                                        groundTruthPositionsTemp[i] = (Convert.ToDouble(groundTruthPositionsPrev6[i]) + ((Convert.ToDouble(groundTruthPositionsToWrite[i]) - Convert.ToDouble(groundTruthPositionsPrev6[i])) * dratio6)).ToString();
                                    }
                                }
                                tempString6 = tempString6 + groundTruthPositionsTemp[i].ToString() + " ";
                            }
                            if (firstWrite6)
                            {
                                firstWrite6 = false;
                            }

                            double y = ((Math.Pow(l26, 2) - Math.Pow(l16, 2) - Math.Pow(d6, 2)) / (2 * d6));
                            double x = (Math.Sqrt(Math.Pow(l16, 2) - Math.Pow(y, 2)));
                            dratio6 = ((Math.Sqrt(Math.Pow(2 * (j + 1) * dradius3, 2) - Math.Pow(x, 2)) - y) / (d6));

                            writer6.WriteLine(tempString6);
                        }
                    }
                    fitableOrb6 = 0;
                    writer6.Close();

                    for (int i = 0; i <= 2; i++)
                    {
                        dvectorStart6[i] = dvectorEnd6[i];
                    }
                }

                //Data from sensors written as it is without any modification
                using (var writer = new StreamWriter(Application.persistentDataPath + "/logs/" + "groundtruthdata" + fileCount + ".txt", append: true))
                {
                    string tempString = null;

                    for (int i = 0; i < 65; i++)
                    {
                        tempString = tempString + groundTruthPositionsToWrite[i].ToString() + " ";
                        groundTruthPositionsPrev4[i] = groundTruthPositionsPrev5[i] = groundTruthPositionsPrev6[i] = groundTruthPositionsToWrite[i];
                    }
                    writer.WriteLine(tempString);
                    writer.Close();
                }
            }
        }
    }
}

﻿// Copyright (c) 2019 Rayaan Ajouz, Bouwen met Staal. Please see the LICENSE file	
// for details. All rights reserved. Use of this source code is governed by a	
// Apache-2.0 license that can be found in the LICENSE file.	
using System;
using System.Linq;
using System.Collections.Generic;

using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using KarambaIDEA.Core;
using KarambaIDEA.IDEA;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace KarambaIDEA
{
    public class CreateIDEAfile : GH_Component
    {
        public CreateIDEAfile() : base("Create IDEA File", "Create IDEA File", "Create IDEA file", "KarambaIDEA", "3. Project utilities")
        {

        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Project", "Project", "Project object of KarambaIdeaCore", GH_ParamAccess.item);
            pManager.AddTextParameter("Output folder ", "Output folder", "Save location of IDEA Statica Connection output file. For example: 'C:\\Data'", GH_ParamAccess.item);
            pManager.AddGenericParameter("Workshop Operations", "WO", "Workshop operations", GH_ParamAccess.item);
            pManager.AddBooleanParameter("RunAllJoints", "RunAllJoints", "If true run all joints, if false run ChooseJoint joint", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ChooseJoint", "ChooseJoint", "Specify the joint that will be calculated in IDEA. Note: starts at zero.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("RunIDEA", "RunIDEA", "Bool for running IDEA Statica Connection", GH_ParamAccess.item);

            // Assign default Workshop Operation.
            Param_GenericObject param0 = (Param_GenericObject)pManager[2];
            param0.PersistentData.Append(new GH_ObjectWrapper(EnumWorkshopOperations.NoOperation));

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Selected Joint", "Selected Joint", "Lines of selected Joint", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Input variables
            Project project = new Project();
            string outputfolderpath = null;
            EnumWorkshopOperations workshopOperations = EnumWorkshopOperations.NoOperation;
            bool calculateAllJoints = false;
            int calculateThisJoint = 0;
            bool startIDEA = false;
                   

            //Link input
            DA.GetData(0, ref project);
            DA.GetData(1, ref outputfolderpath);

            
            DA.GetData(2, ref workshopOperations);

            DA.GetData(3, ref calculateAllJoints);

            DA.GetData(4, ref calculateThisJoint);
            DA.GetData(5, ref startIDEA);
            

            //output variables
            List<Rhino.Geometry.Line> lines = new List<Rhino.Geometry.Line>();
            List<Rhino.Geometry.Line> jointlines = new List<Rhino.Geometry.Line>();

            //Adjust out of bounds index calculateThisJoint
            calculateThisJoint = calculateThisJoint % project.joints.Count;

            if (startIDEA == true)
            {
                project.CreateFolder(outputfolderpath);
                if (calculateAllJoints == true)
                {
                    foreach(Joint joint in project.joints)
                    {
                        joint.workshopOperation = workshopOperations;
                        IdeaConnection ideaConnection = new IdeaConnection(joint);
                    }
                }
                else
                {
                    Joint joint = project.joints[calculateThisJoint];
                    joint.workshopOperation = workshopOperations;
                    IdeaConnection ideaConnection = new IdeaConnection(joint);
                }              
                
                
            }

            //export lines of joint for visualisation purposes
            foreach (int i in project.joints[calculateThisJoint].beamIDs)
            {
                Core.Line line = project.elements[i].line;
                Rhino.Geometry.Line rhiline = ImportGrasshopperUtils.CastLineToRhino(line);
                jointlines.Add(rhiline);
            }
            

            //link output
            DA.SetDataList(0, jointlines);
        }
        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {

                return Properties.Resources.KarambaIDEA_logo;

            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("52308472-3ab8-4f23-89d0-d3746ed013f6"); }
        }


    }

}
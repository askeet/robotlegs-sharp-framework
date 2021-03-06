//------------------------------------------------------------------------------
//  Copyright (c) 2014-2016 the original author or authors. All Rights Reserved. 
// 
//  NOTICE: You are permitted to use, modify, and distribute this file 
//  in accordance with the terms of the license agreement accompanying it. 
//------------------------------------------------------------------------------

﻿using System;
using NUnit.Framework;
using Robotlegs.Bender.Framework.Impl;
using Robotlegs.Bender.Framework.API;
using Robotlegs.Bender.Extensions.ViewManagement;
using Robotlegs.Bender.Extensions.Mediation.API;

namespace Robotlegs.Bender.Extensions.Mediation
{
	public class MediatorMapExtensionTest
	{

		/*============================================================================*/
		/* Private Properties                                                         */
		/*============================================================================*/

		private Context context;

		/*============================================================================*/
		/* Test Setup and Teardown                                                    */
		/*============================================================================*/

		[SetUp]
		public void Setup()
		{
			context = new Context();
		}

		/*============================================================================*/
		/* Tests                                                                      */
		/*============================================================================*/

		[Test]
		public void Installing_After_Initialization_Throws_Error()
		{
            Assert.Throws(typeof(LifecycleException), new TestDelegate(() =>
            {
                context.Initialize();
                context.Install(typeof(MediatorMapExtension));
            }
            ));
		}

		[Test]
		public void MediatorMap_Is_Mapped_Into_Injector_On_Initialize()
		{
			object actual = null;
			context
				.Install(typeof(ViewManagerExtension))
					.Install(typeof(MediatorMapExtension));
			context.WhenInitializing(delegate() {
				actual = context.injector.GetInstance(typeof(IMediatorMap));
			});
			context.Initialize();
			Assert.That(actual, Is.InstanceOf(typeof(IMediatorMap)));
		}

		[Test]
		public void MediatorMap_Is_Unmapped_From_Injector_On_Destroy()
		{
			context
				.Install(typeof(ViewManagerExtension))
				.Install(typeof(MediatorMapExtension));
			context.AfterDestroying( delegate() {
				Assert.That(context.injector.HasMapping(typeof(IMediatorMap)), Is.False);
			});
			context.Initialize();
			context.Destroy();
		}

	}
}


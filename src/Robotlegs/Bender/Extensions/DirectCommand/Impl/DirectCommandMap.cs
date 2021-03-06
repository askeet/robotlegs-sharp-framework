//------------------------------------------------------------------------------
//  Copyright (c) 2014-2016 the original author or authors. All Rights Reserved. 
// 
//  NOTICE: You are permitted to use, modify, and distribute this file 
//  in accordance with the terms of the license agreement accompanying it. 
//------------------------------------------------------------------------------

﻿using System;
using System.Collections.Generic;
using Robotlegs.Bender.Extensions.CommandCenter.API;
using Robotlegs.Bender.Extensions.CommandCenter.Impl;
using Robotlegs.Bender.Framework.API;
using Robotlegs.Bender.Extensions.DirectCommand.API;

namespace Robotlegs.Bender.Extensions.DirectCommand.Impl
{
	public class DirectCommandMap : IDirectCommandMap
	{

		/*============================================================================*/
		/* Private Properties                                                         */
		/*============================================================================*/

		private List<CommandMappingList.Processor> _mappingProcessors = new List<CommandMappingList.Processor>();

		private IContext _context;

		private ICommandExecutor _executor;

		private CommandMappingList _mappings;

		/*============================================================================*/
		/* Constructor                                                                */
		/*============================================================================*/
	
		public DirectCommandMap(IContext context)
		{
			_context = context;
			IInjector sandboxedInjector = context.injector.CreateChild();
			// allow access to this specific instance in the commands
			//sandboxedInjector.map(IDirectCommandMap).toValue(this);
			sandboxedInjector.Map(typeof(IDirectCommandMap)).ToValue(this);
			_mappings = new CommandMappingList(
				new NullCommandTrigger(), _mappingProcessors, context.GetLogger(this));
			_executor = new CommandExecutor(sandboxedInjector, _mappings.RemoveMapping);
		}

		/*============================================================================*/
		/* Public Functions                                                           */
		/*============================================================================*/

		public IDirectCommandConfigurator Map (Type commandClass)
		{
			return new DirectCommandMapper(_executor, _mappings, commandClass);
		}

		public IDirectCommandConfigurator Map <T>()
		{
			return new DirectCommandMapper(_executor, _mappings, typeof(T));
		}

		public void Detain (object command)
		{
			_context.Detain (command);
		}

		public void Release (object command)
		{
			_context.Release (command);
		}

		public void Execute (Robotlegs.Bender.Extensions.CommandCenter.API.CommandPayload payload = null)
		{
			_executor.ExecuteCommands (_mappings.GetList(), payload);
		}

		public IDirectCommandMap AddMappingProcessor (CommandMappingList.Processor handler)
		{
			if (!_mappingProcessors.Contains (handler))
				_mappingProcessors.Add (handler);
			return this;
		}
	}
}


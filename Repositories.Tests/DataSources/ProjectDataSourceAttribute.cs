using Models;
using NUnit.Framework.Interfaces;
using Repositories.Tests.DataGenerators;
using Repositories.Tests.DataGenerators.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Tests.DataSources
{
    /// <summary>
    /// Attribute to generate projects
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ProjectDataSourceAttribute : Attribute, IParameterDataSource
    {
        private readonly IFakeDataGenerator<Project> fakeDataGenerator;
        private readonly int amountToGenerate = 0;

        /// <summary>
        /// Initializes collaboratorDataSourceAttribute
        /// </summary>
        public ProjectDataSourceAttribute()
        {
            fakeDataGenerator = new ProjectDataGenerator();
        }

        /// <summary>
        /// Initializes projectDataSourceAttribute
        /// and setting the amount of projects to be generated
        /// </summary>
        public ProjectDataSourceAttribute(int amount) : this()
        {
            amountToGenerate = amount;
        }

        /// <summary>
        /// Generate the data and return it
        /// </summary>
        /// <param name="parameter">Extra parameters given in the attribute, not in use but required due to inheritance</param>
        /// <returns>The generated data</returns>
        public IEnumerable GetData(IParameterInfo parameter)
        {
            if(amountToGenerate <= 1)
            {
                return new[] { fakeDataGenerator.Generate() };
            }
            List<Project> projects = fakeDataGenerator.GenerateRange(amountToGenerate).ToList();
            return new[] { projects };
        }
    }
}

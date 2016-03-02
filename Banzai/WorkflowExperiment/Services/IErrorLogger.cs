using System;

namespace WorkflowExperiment.Services
{
    public interface IErrorLogger
    {
        void Exception(Exception e);
    }
}

using ApprovalTests.Namers;
using ApprovalTests.Reporters;

[assembly: UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]

#if(IE)
[assembly:UseApprovalSubdirectory("IE")]
#endif
#if(V8)
[assembly: UseApprovalSubdirectory("V8")]
#endif
#if(Jint)
[assembly:UseApprovalSubdirectory("Jint")]
#endif
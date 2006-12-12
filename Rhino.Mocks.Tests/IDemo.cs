using System;

namespace Rhino.Mocks.Tests
{
	public interface IDemo
	{
        string ReadOnly { get; }
        string WriteOnly { get; }
        string Prop { get; set; }
		Enum EnumNoArgs();
		void VoidNoArgs();
		string StringArgString(string arg2);
		string ReturnStringNoArgs();
		void VoidStringArg(string arg1);
		int ReturnIntNoArgs();
		void VoidThreeArgs(int i, string s, float f );
		void VoidThreeStringArgs(string s1, string s2, string s3);
		void VoidValueTypeArrayArgs( short[] u );
	}

	internal enum EnumDemo
	{
		One,
		Two,
		Three,
		Dozen
	}
}
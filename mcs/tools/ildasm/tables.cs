using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono;

/*
 * This partial class contains the tables used by the disassembler.
 */
public partial class ILDAsm {

	Dictionary<uint, string> type_access_map = new Dictionary <uint, string> () {
		{ (uint)TypeAttributes.NotPublic, "private " },
		{ (uint)TypeAttributes.Public, "public " },
		{ (uint)TypeAttributes.NestedPublic, "nested public " },
		{ (uint)TypeAttributes.NestedPrivate, "nested private " },
		{ (uint)TypeAttributes.NestedFamily, "nested family " },
		{ (uint)TypeAttributes.NestedAssembly, "nested assembly " },
		{ (uint)TypeAttributes.NestedFamANDAssem, "nested famandassem " },
		{ (uint)TypeAttributes.NestedFamORAssem, "nested famorassem " },
	};
	Dictionary<uint, string> type_layout_map = new Dictionary <uint, string> () {
		{ (uint)TypeAttributes.AutoLayout, "auto " },
		{ (uint)TypeAttributes.SequentialLayout, "sequential " },
		{ (uint)TypeAttributes.ExplicitLayout, "explicit " }
	};
	Dictionary<uint, string> type_string_format_map = new Dictionary <uint, string> () {
		{ (uint)TypeAttributes.AnsiClass, "ansi " },
		{ (uint)TypeAttributes.UnicodeClass, "unicode " },
		//{ (uint)TypeAttributes.AutoClass, "unicode " },
	};
	Dictionary<uint, string> type_flag_map = new Dictionary <uint, string> () {
		{ (uint)TypeAttributes.Abstract, "abstract " },
		{ (uint)TypeAttributes.Sealed, "sealed " },
		{ (uint)TypeAttributes.SpecialName, "specialname " },
		{ (uint)TypeAttributes.Import, "import " },
		{ (uint)TypeAttributes.Serializable, "serializable " },
		{ (uint)TypeAttributes.BeforeFieldInit, "beforefieldinit " },
		{ (uint)TypeAttributes.RTSpecialName, "rtspecialname " },
		//{ (uint)TypeAttributes.HasSecurity, "rtspecialname " },
		//{ (uint)TypeAttributes.Forwarder, "rtspecialname " },
	};

	Dictionary<uint, string> type_sem_map = new Dictionary <uint, string> () {
		{ (uint)TypeAttributes.Class, "" },
		{ (uint)TypeAttributes.Interface, "interface " },
	};


	Dictionary<uint, string> field_access_map = new Dictionary <uint, string> () {
		{ (uint)FieldAttributes.Private, "private " },
		{ (uint)FieldAttributes.FamANDAssem, "famandassem " },
		{ (uint)FieldAttributes.Assembly, "assembly " },
		{ (uint)FieldAttributes.Family, "family " },
		{ (uint)FieldAttributes.FamORAssem, "famorassem " },
		{ (uint)FieldAttributes.Public, "public " },
	};

	Dictionary<uint, string> field_flag_map = new Dictionary <uint, string> () {
		{ (uint)FieldAttributes.Static, "static " },
		{ (uint)FieldAttributes.Literal, "literal " },
		{ (uint)FieldAttributes.InitOnly, "initonly " },
		{ (uint)FieldAttributes.SpecialName, "specialname " },
		{ (uint)FieldAttributes.RTSpecialName, "rtspecialname " },
		{ (uint)FieldAttributes.NotSerialized, "notserialized " },		
	};


	Dictionary<uint, string> method_access_map = new Dictionary <uint, string> () {
		{ (uint)MethodAttributes.CompilerControlled, "privatescope " },
		{ (uint)MethodAttributes.Private, "private " },
		{ (uint)MethodAttributes.Public, "public " },
		{ (uint)MethodAttributes.Family, "family " },
		{ (uint)MethodAttributes.FamORAssem, "famorassem " },
		{ (uint)MethodAttributes.Assembly, "assembly " }
	};

	Dictionary<uint, string> method_flag_map = new Dictionary <uint, string> () {
		{ (uint)MethodAttributes.Static, "static " },
		{ (uint)MethodAttributes.Final, "final " },
		{ (uint)MethodAttributes.Virtual, "virtual " },
		{ (uint)MethodAttributes.HideBySig, "hidebysig " },
		{ (uint)MethodAttributes.NewSlot, "newslot " },
		{ (uint)MethodAttributes.CheckAccessOnOverride, "strict " },
		{ (uint)MethodAttributes.Abstract, "abstract " },
		{ (uint)MethodAttributes.SpecialName, "specialname " },
		{ (uint)MethodAttributes.RTSpecialName, "rtspecialname " },
	};


	Dictionary<uint, string> method_impl_map = new Dictionary <uint, string> () {
		{ (uint)MethodImplAttributes.IL, "cil " },
		{ (uint)MethodImplAttributes.Runtime, "runtime " }
	};
	Dictionary<uint, string> method_managed_map = new Dictionary <uint, string> () {
		{ (uint)MethodImplAttributes.Managed, "managed " }
	};
	Dictionary<uint, string> method_impl_flag_map = new Dictionary <uint, string> () {
		{ (uint)MethodImplAttributes.PreserveSig, "preservesig " },
		{ (uint)MethodImplAttributes.InternalCall, "internalcall " },
		{ (uint)MethodImplAttributes.Synchronized, "synchronized " },
		{ (uint)MethodImplAttributes.NoInlining, "noinlining " },
	};


	Dictionary<NativeType, string> native_type_to_str = new Dictionary<NativeType, string> () {
		{ NativeType.Boolean, "bool" },
		{ NativeType.I1, "int8" },
		{ NativeType.U1, "unsigned int8" },
		{ NativeType.I2, "int16" },
		{ NativeType.U2, "unsigned int16" },
		{ NativeType.I4, "int32" },
		{ NativeType.U4, "unsigned int32" },
		{ NativeType.I8, "int64" },
		{ NativeType.U8, "unsigned int64" },
		//{ NativeType.R4, "int32" },
		//{ NativeType.R8, "unsigned int32" },
		{ NativeType.LPStr, "lpstr" },
		{ NativeType.Int, "int" },
		//{ NativeType.Int, "int" },
		{ NativeType.Func, "method" },
		//{ NativeType.Array, "int" },
		//{ NativeType.Currency, "method" },
		{ NativeType.BStr, "bstr " },
		{ NativeType.LPWStr, "lpwstr" },
		//{ NativeType.LPTStr, "lpwstr" },
		//{ NativeType.FixedSysString, "lpwstr" },
		{ NativeType.IUnknown, "iunknown" },
		{ NativeType.IDispatch, "idispatch" },
		{ NativeType.Struct, "struct" },
		{ NativeType.IntF, "interface" },
		//{ NativeType.SafeArray, "interface" },
		//{ NativeType.FixedArray, "interface" },
		//{ NativeType.ByValStr, "interface" },
		//{ NativeType.ANSIBStr, "interface" },
		//{ NativeType.TBStr, "interface" },
		{ NativeType.VariantBool, "variant bool" },
		{ NativeType.ASAny, "as any" },
		{ NativeType.LPStruct, "lpstruct" },
		//{ NativeType.CustomMarshaler, "lpstruct" },
		//{ NativeType.Error, "lpstruct" },
	};


	Dictionary<uint, string> pinvoke_char_set_map = new Dictionary <uint, string> () {
		{ (uint)PInvokeAttributes.CharSetNotSpec, "" },
		{ (uint)PInvokeAttributes.CharSetAnsi, "ansi " },
		{ (uint)PInvokeAttributes.CharSetUnicode, "unicode " },
		{ (uint)PInvokeAttributes.CharSetAuto, "autochar " },
	};

	Dictionary<uint, string> pinvoke_cconv_map = new Dictionary <uint, string> () {
		{ (uint)PInvokeAttributes.CallConvWinapi, "winapi " },
		{ (uint)PInvokeAttributes.CallConvCdecl, "cdecl " },
		{ (uint)PInvokeAttributes.CallConvStdCall, "stdcall " },
		{ (uint)PInvokeAttributes.CallConvThiscall, "thiscall " },
		{ (uint)PInvokeAttributes.CallConvFastcall, "fastcall " },
	};

	Dictionary<uint, string> pinvoke_flags_map = new Dictionary <uint, string> () {
		{ (uint)PInvokeAttributes.SupportsLastError, "lasterr " },
	};


	Dictionary<SecurityAction, string> sec_action_to_string = new Dictionary<SecurityAction, string> () {
		//{ SecurityAction.Request, "reqmin" },
		{ SecurityAction.Demand, "demand" },
		{ SecurityAction.Assert, "assert" },
		{ SecurityAction.Deny, "deny" },
		{ SecurityAction.PermitOnly, "permitonly" },
		{ SecurityAction.LinkDemand, "linkcheck" },
		{ SecurityAction.InheritDemand, "inheritcheck" },
		{ SecurityAction.RequestMinimum, "reqmin" },
		{ SecurityAction.RequestOptional, "reqopt" },
		{ SecurityAction.RequestRefuse, "reqrefuse" },
		//{ SecurityAction.PreJitGrant, "reqmin" },
		//{ SecurityAction.PreJitDeny, "reqmin" },
		//{ SecurityAction.NonCasDemand, "reqmin" },
		//{ SecurityAction.NonCasLinkDemand, "reqmin" },
		//{ SecurityAction.NonCasInheritance, "reqmin" },
	};


	Dictionary<string, bool> keyword_table = new Dictionary<string, bool> {
		{"9", true},
		{"abstract", true},
		{"add", true},
		{"add.ovf", true},
		{"add.ovf.un", true},
		{"algorithm", true},
		{"alignment", true},
		{"and", true},
		{"ansi", true},
		{"any", true},
		{"arglist", true},
		{"array", true},
		{"as", true},
		{"assembly", true},
		{"assert", true},
		{"at", true},
		{"autochar", true},
		{"auto", true},
		{"beforefieldinit", true},
		{"bestfit", true},
		{"beq", true},
		{"beq.s", true},
		{"bge", true},
		{"bge.s", true},
		{"bge.un", true},
		{"bge.un.s", true},
		{"bgt", true},
		{"bgt.s", true},
		{"bgt.un", true},
		{"bgt.un.s", true},
		{"ble", true},
		{"ble.s", true},
		{"ble.un", true},
		{"ble.un.s", true},
		{"blob", true},
		{"blob_object", true},
		{"blt", true},
		{"blt.s", true},
		{"blt.un", true},
		{"blt.un.s", true},
		{"bne.un", true},
		{"bne.un.s", true},
		{"bool", true},
		{"box", true},
		{"break", true},
		{"brfalse", true},
		{"brfalse.s", true},
		{"br", true},
		{"brinst", true},
		{"brinst.s", true},
		{"brnull", true},
		{"brnull.s", true},
		{"br.s", true},
		{"brtrue", true},
		{"brtrue.s", true},
		{"brzero", true},
		{"brzero.s", true},
		{"bstr", true},
		{"bytearray", true},
		{"byvalstr", true},
		{"call", true},
		{"callconv", true},
		{"calli", true},
		{"callmostderived", true},
		{"callvirt", true},
		{"carray", true},
		{"castclass", true},
		{"catch", true},
		{"cdecl", true},
		{"ceq", true},
		{"cf", true},
		{"cgt", true},
		{"cgt.un", true},
		{"char", true},
		{"charmaperror", true},
		{"cil", true},
		{"ckfinite", true},
		{"class", true},
		{"clsid", true},
		{"clt", true},
		{"clt.un", true},
		{"Compilercontrolled", true},
		{"const", true},
		{"conv.i1", true},
		{"conv.i2", true},
		{"conv.i4", true},
		{"conv.i8", true},
		{"conv.i", true},
		{"conv.ovf.i1", true},
		{"conv.ovf.i1.un", true},
		{"conv.ovf.i2", true},
		{"conv.ovf.i2.un", true},
		{"conv.ovf.i4", true},
		{"conv.ovf.i4.un", true},
		{"conv.ovf.i8", true},
		{"conv.ovf.i8.un", true},
		{"conv.ovf.i", true},
		{"conv.ovf.i.un", true},
		{"conv.ovf.u1", true},
		{"conv.ovf.u1.un", true},
		{"conv.ovf.u2", true},
		{"conv.ovf.u2.un", true},
		{"conv.ovf.u4", true},
		{"conv.ovf.u4.un", true},
		{"conv.ovf.u8", true},
		{"conv.ovf.u8.un", true},
		{"conv.ovf.u", true},
		{"conv.ovf.u.un", true},
		{"conv.r4", true},
		{"conv.r8", true},
		{"conv.r.un", true},
		{"conv.u1", true},
		{"conv.u2", true},
		{"conv.u4", true},
		{"conv.u8", true},
		{"conv.u", true},
		{"cpblk", true},
		{"cpobj", true},
		{"currency", true},
		{"custom", true},
		{"date", true},
		{"decimal", true},
		{"default", true},
		{"demand", true},
		{"deny", true},
		{"div", true},
		{"div.un", true},
		{"dup", true},
		{"endfault", true},
		{"endfilter", true},
		{"endfinally", true},
		{"endmac", true},
		{"enum", true},
		{"error", true},
		{"explicit", true},
		{"extends", true},
		{"extern", true},
		{"false", true},
		{"famandassem", true},
		{"family", true},
		{"famorassem", true},
		{"fastcall", true},
		{"fault", true},
		{"field", true},
		{"filetime", true},
		{"filter", true},
		{"final", true},
		{"finally", true},
		{"fixed", true},
		{"flags", true},
		{"float32", true},
		{"float64", true},
		{"float", true},
		{"forwardref", true},
		{"fromunmanaged", true},
		{"handler", true},
		{"hidebysig", true},
		{"hresult", true},
		{"idispatch", true},
		{"il", true},
		{"illegal", true},
		{"implements", true},
		{"implicitcom", true},
		{"implicitres", true},
		{"import", true},
		{"in", true},
		{"inheritcheck", true},
		{"initblk", true},
		{"init", true},
		{"initobj", true},
		{"initonly", true},
		{"instance", true},
		{"int16", true},
		{"int32", true},
		{"int64", true},
		{"int8", true},
		{"interface", true},
		{"internalcall", true},
		{"int", true},
		{"isinst", true},
		{"iunknown", true},
		{"jmp", true},
		{"lasterr", true},
		{"lcid", true},
		{"ldarg.0", true},
		{"ldarg.1", true},
		{"ldarg.2", true},
		{"ldarg.3", true},
		{"ldarga", true},
		{"ldarga.s", true},
		{"ldarg", true},
		{"ldarg.s", true},
		{"ldc.i4.0", true},
		{"ldc.i4.1", true},
		{"ldc.i4.2", true},
		{"ldc.i4.3", true},
		{"ldc.i4.4", true},
		{"ldc.i4.5", true},
		{"ldc.i4.6", true},
		{"ldc.i4.7", true},
		{"ldc.i4.8", true},
		{"ldc.i4", true},
		{"ldc.i4.m1", true},
		{"ldc.i4.M1", true},
		{"ldc.i4.s", true},
		{"ldc.i8", true},
		{"ldc.r4", true},
		{"ldc.r8", true},
		{"ldelem", true},
		{"ldelema", true},
		{"ldelem.i1", true},
		{"ldelem.i2", true},
		{"ldelem.i4", true},
		{"ldelem.i8", true},
		{"ldelem.i", true},
		{"ldelem.r4", true},
		{"ldelem.r8", true},
		{"ldelem.ref", true},
		{"ldelem.u1", true},
		{"ldelem.u2", true},
		{"ldelem.u4", true},
		{"ldelem.u8", true},
		{"ldflda", true},
		{"ldfld", true},
		{"ldftn", true},
		{"ldind.i1", true},
		{"ldind.i2", true},
		{"ldind.i4", true},
		{"ldind.i8", true},
		{"ldind.i", true},
		{"ldind.r4", true},
		{"ldind.r8", true},
		{"ldind.ref", true},
		{"ldind.u1", true},
		{"ldind.u2", true},
		{"ldind.u4", true},
		{"ldind.u8", true},
		{"ldlen", true},
		{"ldloc.0", true},
		{"ldloc.1", true},
		{"ldloc.2", true},
		{"ldloc.3", true},
		{"ldloca", true},
		{"ldloca.s", true},
		{"ldloc", true},
		{"ldloc.s", true},
		{"ldnull", true},
		{"ldobj", true},
		{"ldsflda", true},
		{"ldsfld", true},
		{"ldstr", true},
		{"ldtoken", true},
		{"ldvirtftn", true},
		{"leave", true},
		{"leave.s", true},
		{"legacy", true},
		{"linkcheck", true},
		{"literal", true},
		{"localloc", true},
		{"lpstr", true},
		{"lpstruct", true},
		{"lptstr", true},
		{"lpvoid", true},
		{"lpwstr", true},
		{"managed", true},
		{"marshal", true},
		{"method", true},
		{"mkrefany", true},
		{"modopt", true},
		{"modreq", true},
		{"mul", true},
		{"mul.ovf", true},
		{"mul.ovf.un", true},
		{"native", true},
		{"neg", true},
		{"nested", true},
		{"newarr", true},
		{"newobj", true},
		{"newslot", true},
		{"noappdomain", true},
		{"noinlining", true},
		{"nomachine", true},
		{"nomangle", true},
		{"nometadata", true},
		{"noncasdemand", true},
		{"noncasinheritance", true},
		{"noncaslinkdemand", true},
		{"nop", true},
		{"noprocess", true},
		{"not", true},
		{"not_in_gc_heap", true},
		{"notremotable", true},
		{"notserialized", true},
		{"null", true},
		{"nullref", true},
		{"object", true},
		{"objectref", true},
		{"off", true},
		{"on", true},
		{"opt", true},
		{"optil", true},
		{"or", true},
		{"out", true},
		{"permitonly", true},
		{"pinned", true},
		{"pinvokeimpl", true},
		{"pop", true},
		{"prefix1", true},
		{"prefix2", true},
		{"prefix3", true},
		{"prefix4", true},
		{"prefix5", true},
		{"prefix6", true},
		{"prefix7", true},
		{"prefixref", true},
		{"prejitdeny", true},
		{"prejitgrant", true},
		{"preservesig", true},
		{"private", true},
		{"privatescope", true},
		{"property", true},
		{"protected", true},
		{"public", true},
		{"readonly", true},
		{"record", true},
		{"refany", true},
		{"refanytype", true},
		{"refanyval", true},
		{"rem", true},
		{"rem.un", true},
		{"reqmin", true},
		{"reqopt", true},
		{"reqrefuse", true},
		{"reqsecobj", true},
		{"request", true},
		{"ret", true},
		{"rethrow", true},
		{"retval", true},
		{"rtspecialname", true},
		{"runtime", true},
		{"safearray", true},
		{"sealed", true},
		{"sequential", true},
		{"serializable", true},
		{"shl", true},
		{"shr", true},
		{"shr.un", true},
		{"sizeof", true},
		{"special", true},
		{"specialname", true},
		{"starg", true},
		{"starg.s", true},
		{"static", true},
		{"stdcall", true},
		{"stelem", true},
		{"stelem.i1", true},
		{"stelem.i2", true},
		{"stelem.i4", true},
		{"stelem.i8", true},
		{"stelem.i", true},
		{"stelem.r4", true},
		{"stelem.r8", true},
		{"stelem.ref", true},
		{"stfld", true},
		{"stind.i1", true},
		{"stind.i2", true},
		{"stind.i4", true},
		{"stind.i8", true},
		{"stind.i", true},
		{"stind.r4", true},
		{"stind.r8", true},
		{"stloc", true},
		{"stobj", true},
		{"storage", true},
		{"stored_object", true},
		{"streamed_object", true},
		{"stream", true},
		{"strict", true},
		{"string", true},
		{"struct", true},
		{"stsfld", true},
		{"sub", true},
		{"sub.ovf", true},
		{"sub.ovf.un", true},
		{"switch", true},
		{"synchronized", true},
		{"syschar", true},
		{"sysstring", true},
		{"tbstr", true},
		{"thiscall", true},
		{"tls", true},
		{"to", true},
		{"true", true},
		{"type", true},
		{"typedref", true},
		{"uint", true},
		{"uint8", true},
		{"uint16", true},
		{"uint32", true},
		{"uint64", true},
		{"unbox", true},
		{"unicode", true},
		{"unmanagedexp", true},
		{"unmanaged", true},
		{"unsigned", true},
		{"userdefined", true},
		{"value", true},
		{"valuetype", true},
		{"vararg", true},
		{"variant", true},
		{"vector", true},
		{"virtual", true},
		{"void", true},
		{"wchar", true},
		{"winapi", true},
		{"with", true},
		{"xor", true},
	};
}
/*
	dummy.h
*/

#ifndef UT_DUMMY
#define UT_DUMMY

#ifndef UT_DUMMY_DEF
#define UT_DUMMY_EXT extern
#else
#define UT_DUMMY_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	単体テスト用設定															*/
/*------------------------------------------------------------------------------*/
#include "Platform_Types.h"
#include "Compiler_Cfg.h"
#include "Compiler.h"

#include "Rte_swc_in_oilp.h"
#include "bsw_common.h"
#include "Rte_swc_in_oilp_map.h"

#define swc_in_oilp_CODE



#endif	/* UT_DUMMY */

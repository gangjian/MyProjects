/*	$RCSfile: bsw_common.h $										*/
/*	$Date: 2015/11/06 19:38:22JST $									*/
/*	$Revision: 1.3 $												*/
/*	 EXPLANATION: BSW共通ヘッダファイル								*/

#ifndef BSW_COMMON_INC
#define BSW_COMMON_INC

#include "std_lib.h"			/*	BSW標準ライブラリ 公開ヘッダファイル			*/

#include "bsw.h"				/*	BSW用ヘッダファイル								*/

#include "bsw_in_csmon.h"		/*	BSW CSモニタ 公開ヘッダファイル					*/

#include "bsw_common_map.h"		/*	BSW共通コンフィグレーションファイル				*/

#include "bsw_common_def.h"		/*	BSW共通定義 公開ヘッダファイル					*/
#include "bsw_common_comdef.h"	/*	BSW共通定義(制御目標値(com)) 公開ヘッダファイル	*/
#include "bsw_common_nvdef.h"	/*	BSW共通定義(不揮発性メモリ) 公開ヘッダファイル	*/

#endif	/*	BSW_COMMON_INC	*/

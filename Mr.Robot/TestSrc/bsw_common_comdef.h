/*	$RCSfile: bsw_common_comdef.h $									*/
/*	$Date: 2015/11/09 13:26:54JST $									*/
/*	$Revision: 1.4 $												*/
/*	 EXPLANATION: BSW共通定義(制御目標値(com)) 公開ヘッダファイル	*/

#ifndef BSW_COMMON_COMDEF_INC
#define BSW_COMMON_COMDEF_INC

#ifndef BSW_COMMON_COMDEF_DEF
#define BSW_COMMON_COMDEF_EXT extern
#else
#define BSW_COMMON_COMDEF_EXT
#endif

/*------------------------------------------------------------------------------*/
/*	マクロ定義(変更禁止)														*/
/*------------------------------------------------------------------------------*/
/* 信号有無設定 */
#define COMMON_COMDEF_SIG_NONEXIST		(0)								/* 情報量別の信号名有無：無し				*/
#define COMMON_COMDEF_SIG_EXIST			(1)								/* 情報量別の信号名有無：有り				*/

/*----------------------------------------------------------*/
/* 信号ID有無（変更可）										*/
/* REMOTE_UINT8,uint8,uint16,uint32毎に信号ID有無を設定		*/
/*----------------------------------------------------------*/
#define COMMON_COMDEF_SIG_RW_UINT8		(COMMON_COMDEF_SIG_NONEXIST)	/* リモートウォーニング信号ID有無			*/
#define COMMON_COMDEF_SIG_UINT8			(COMMON_COMDEF_SIG_EXIST)		/* 情報量uint8の信号ID有無					*/
#define COMMON_COMDEF_SIG_UINT16		(COMMON_COMDEF_SIG_EXIST)		/* 情報量uint16の信号ID有無					*/
#define COMMON_COMDEF_SIG_UINT32		(COMMON_COMDEF_SIG_EXIST)		/* 情報量uint32の信号ID有無					*/


/********************/
/*	各データ定義	*/
/********************/
/*------------------------------------------*/
/*	通常制御目標値用定義					*/
/*	アクティブテスト制御目標値用定義		*/
/*------------------------------------------*/
/*	リモートウォーニング制御目標値	*/
#ifndef	APP_UNUSED_OUT_SIG_RW_ALL
#define TARGET_COM_DT_RW_OFF				((uint8) 0U)		/*	表示要求無										*/
#define TARGET_COM_DT_RW_ON					((uint8) 1U)		/*	表示要求有										*/
#endif	/*	APP_UNUSED_OUT_SIG_RW_ALL	*/

/* ↓ユーザー編集可能↓ */

/*	ポーリング送信要求制御目標(METD_MST)	*/
#define TARGET_COM_DT_DIAGCOM_TARGET_NG		((uint8) 0U)		/*	ポーリング送信要求:無							*/
#define TARGET_COM_DT_DIAGCOM_TARGET_NORMAL	((uint8) 1U)		/*	ポーリング送信要求:通常ダイアグ					*/
#define TARGET_COM_DT_DIAGCOM_TARGET_CMPL	((uint8) 2U)		/*	ポーリング送信要求:強制ダイアグ					*/

/*	送信車速制御目標値(MET_SPD)	*/
#define TARGET_COM_DT_SPD_INIT				((uint8) 0U)		/*	送信車速初期値									*/

/*	現在輝度値(レオスタット)制御目標値(RHEOSTAT)	*/
#define TARGET_COM_DT_ILL_RHEOS_DUTY_MIN	((uint8) 0U)		/*	現在輝度値(レオスタット)の最小値(LSB:1(%))		*/
#define TARGET_COM_DT_ILL_RHEOS_DUTY_MAX	((uint8) 100U)		/*	現在輝度値(レオスタット)の最大値(LSB:1(％))		*/

/*	テールキャンセル信号制御目標値(TAIL_CN)	*/
#define TARGET_COM_DT_TAIL_CN_UNKNOWN		((uint8) 0U)		/*	SW状態不確定									*/
#define TARGET_COM_DT_TAIL_CN_UNFIX			((uint8) 1U)		/*	未定義											*/
#define TARGET_COM_DT_TAIL_CN_ON			((uint8) 2U)		/*	テールキャンセル状態(ON)						*/
#define TARGET_COM_DT_TAIL_CN_OFF			((uint8) 3U)		/*	非テールキャンセル状態(OFF)						*/

/*	ラウンジイルミ消灯信号制御目標値(ILL_OF)	*/
#define TARGET_COM_DT_ILL_OF_ON				((uint8) 0U)		/*	点灯指示										*/
#define TARGET_COM_DT_ILL_OF_OFF			((uint8) 1U)		/*	消灯指示										*/

/*	外気温度送信単位制御目標値(UNIT_TMP)	*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_UNKNOWN1	((uint8) 0U)	/* 未確定1									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_UNKNOWN2	((uint8) 0U)	/* 未確定2									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_CNTGRD		((uint8) 0U)	/* 摂氏(℃)									*/
#define TARGET_COM_DT_OSTEMP2_TARGET_COMUNIT_FHRNHT		((uint8) 0U)	/* 華氏(゜F)								*/

/*	ドライブモニタ単位信号0制御目標値(UNIT_0)	*/
/*	定義無し								*/

/*	スピード公差情報制御目標値(SP_TL)	*/
#define TARGET_COM_DT_TOLER_UNDEF			((uint8) 0U)		/*	未定義											*/

/*	仕向地情報制御目標値(MET_DEST)	*/
#define TARGET_COM_DT_DEST_NON				((uint8) 240U)		/*	仕向地情報無し									*/

/*	ハンドル情報制御目標値(R_L)	*/
#define TARGET_COM_DT_HANDLE_NON			((uint8) 0U)		/* ハンドル情報無し									*/
																/* 	(通信初期時、EEPROM故障時、ハンドル左右共通時)	*/
#define TARGET_COM_DT_HANDLE_R				((uint8) 1U)		/* 右ハンドル車										*/
#define TARGET_COM_DT_HANDLE_L				((uint8) 2U)		/* 左ハンドル車										*/
#define TARGET_COM_DT_HANDLE_UNDEF			((uint8) 3U)		/* ハンドル情報無し(未定義)							*/

/*	オド単位(通信)制御目標値(ODO_UNIT)	*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_UNKNOWN			((uint8) 0U)	/*	オド送信単位：単位情報無し				*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_KM				((uint8) 1U)	/*	オド送信単位：KM						*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_MILE			((uint8) 2U)	/*	オド送信単位：MILE						*/
#define	TARGET_COM_DT_ODTR_ODO_UNIT_COM_UNKNOWN2		((uint8) 3U)	/*	オド送信単位：単位情報無し				*/

/*	ドライブモニタ単位信号1制御目標値(UNIT_1)	*/
/*	定義無し								*/

/*	ドライブモニタ単位信号2制御目標値(UNIT_2)	*/
/*	定義無し								*/

/*	ドライブモニタ単位信号3制御目標値(UNIT_3)	*/
/*	定義無し								*/

/*	燃費表示スケール(FC_SCL)	*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN		((uint8)0x00)		/*	未定義											*/
#define TARGET_COM_DT_FC_SCL_15KPL			((uint8)0x01)		/*	15km/L											*/
#define TARGET_COM_DT_FC_SCL_20KPL			((uint8)0x02)		/*	20km/L											*/
#define TARGET_COM_DT_FC_SCL_30KPL			((uint8)0x03)		/*	30km/L 1										*/
#define TARGET_COM_DT_FC_SCL_40KPL			((uint8)0x04)		/*	40km/L											*/
#define TARGET_COM_DT_FC_SCL_30KPL2			((uint8)0x05)		/*	30km/L 2										*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN2		((uint8)0x06)		/*	未定義2											*/
#define TARGET_COM_DT_FC_SCL_UNKNOWN3		((uint8)0x07)		/*	未定義3											*/

/*	送信車速制御目標値(ゲージ)	*/
#define TARGET_COM_DT_SP_MIN				((uint8) 0U)		/* 送信車速最小値									*/
#define TARGET_COM_DT_SP_MAX				((uint8) 255U)		/* 送信車速最大値									*/


/*	ドライブモニタ単位信号4制御目標値(UNIT_4)	*/
/*	定義無し								*/

/*	ドライブモニタ単位信号5制御目標値(UNIT_5)	*/
/*	定義無し								*/

/*	ドライブモニタ単位信号6制御目標値(UNIT_6)	*/
/*	定義無し								*/

/*	フューエルゲージ状態制御目標値(FUGAGE_S)	*/
#define TARGET_COM_DT_FUGAGES_UNFIX 		((uint8) 0U)		/* 未確定											*/
#define TARGET_COM_DT_FUGAGES_FIXED			((uint8) 1U)		/* 確定												*/

/*	フューエルゲージ指示値制御目標値(FUGAGE)	*/
#define TARGET_COM_DT_FUGAGE_MIN			((uint8) 0U)		/*	Fuelゲージ指示値信号最小値						*/
#define TARGET_COM_DT_FUGAGE_MAX			((uint8) 255U)		/*	Fuelゲージ指示値信号最大値						*/

/*	オイルメンテナンス距離制御目標値(OM_MLG)	*/
/*	定義無し									*/

/*	プレオイルメンテナンスフラグ制御目標値(PR_OM_FL)	*/
/*	定義無し											*/

/*	ターンランプ状態制御目標値(TNS)	*/
#define TARGET_COM_DT_TNS_UNDEF				((uint8) 0U)		/*	未定義											*/
#define TARGET_COM_DT_TNS_TURNL				((uint8) 1U)		/*	ターンランプL動作中								*/
#define TARGET_COM_DT_TNS_TURNR				((uint8) 2U)		/*	ターンランプR動作中								*/
#define TARGET_COM_DT_TNS_OFF				((uint8) 3U)		/*	ターンランプ非動作								*/

/*	ハザード入力状態制御目標値(HZS)	*/
#define TARGET_COM_DT_HZS_OFF				((uint8) 0U)		/*	ハザードSW=OFF									*/
#define TARGET_COM_DT_HZS_ON				((uint8) 1U)		/*	ハザードSW=ON									*/

/*	単位切替信号2(KM/MILE)制御目標値(UNIT_CH2)	*/
#define TARGET_COM_DT_UNIT_NONE				((uint8) 0U)		/*	単位表示なし									*/
#define TARGET_COM_DT_UNIT_KMH				((uint8) 1U)		/*	km/h											*/
#define TARGET_COM_DT_UNIT_MPH				((uint8) 2U)		/*	mph												*/

/*	速度公差A制御目標値(TOLER_A)	*/
/*	定義無し(リニア値の為)			*/

/*	速度公差B制御目標値(TOLER_B)	*/
/*	定義無し(リニア値の為)			*/

/*	言語設定制御目標値(M_LANG)	*/
/*	定義無し					*/

/*	言語連携状態制御目標値(M_LNG_ST)	*/
/*	定義無し							*/

/*	表示可能言語一覧1制御目標値(M_LNGDB1)	*/
/*	定義無し								*/

/*	表示可能言語一覧2制御目標値(M_LNGDB2)	*/
/*	定義無し								*/

/*	表示可能言語一覧3制御目標値(M_LNGDB3)	*/
/*	定義無し								*/

/*	表示可能言語一覧4制御目標値(M_LNGDB4)	*/
/*	定義無し								*/

/*	表示可能言語一覧5制御目標値(M_LNGDB5)	*/
/*	定義無し								*/

/*	表示可能言語一覧6制御目標値(M_LNGDB6)	*/
/*	定義無し								*/

/*	表示可能言語一覧7制御目標値(M_LNGDB7)	*/
/*	定義無し								*/

/*	設定可能単位一覧１制御目標値(M_UNTDB1)	*/
/*	定義無し								*/

/*	LDA警報タイミングステアリングSW(Enter)操作制御目標値(LDAMCUS)	*/
#define TARGET_COM_DT_LDAMCUS_UNDEF			((uint8) 0U)		/*	未定義											*/
#define TARGET_COM_DT_LDAMCUS_HIGH			((uint8) 1U)		/*	High											*/
#define TARGET_COM_DT_LDAMCUS_STANDARD		((uint8) 2U)		/*	Standard										*/

/*	SWS感度ステアリングSW(Enter)操作制御目標値(FCMMCUS)	*/
#define TARGET_COM_DT_FCMMCUS_UNDEF			((uint8) 0U)		/*	未定義											*/
#define TARGET_COM_DT_FCMMCUS_HIGH			((uint8) 1U)		/*	High											*/
#define TARGET_COM_DT_FCMMCUS_NORMAL		((uint8) 2U)		/*	Normal											*/
#define TARGET_COM_DT_FCMMCUS_LOW			((uint8) 3U)		/*	Low												*/

/*	SWS ON/OFFステアリングSW(Enter)操作制御目標値(FCMMSW)	*/
#define TARGET_COM_DT_FCMMSW_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_FCMMSW_ON				((uint8) 1U)		/*	ソフトSW操作									*/

/*	RCTA ON/OFFステアリングSW(Enter)操作制御目標値(RCTAMSW)	*/
#define TARGET_COM_DT_RCTAMSW_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_RCTAMSW_ON			((uint8) 1U)		/*	ソフトSW操作									*/

/*	BSM ON/OFFステアリングSW(Enter)操作制御目標値(BSMMSW)	*/
#define TARGET_COM_DT_BSMMSW_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_BSMMSW_ON				((uint8) 1U)		/*	ソフトSW操作									*/

/*	PCS感度切替ステアリングSW(Enter)操作制御目標値(PCSMCUS)	*/
#define TARGET_COM_DT_PCSMCUS_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_PCSMCUS_ON			((uint8) 1U)		/*	ソフトSW操作									*/

/*	PCS ON/OFFステアリングSW(Enter)操作制御目標値(PCSMSW)	*/
#define TARGET_COM_DT_PCSMSW_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_PCSMSW_ON				((uint8) 1U)		/*	ソフトSW操作									*/

/*	TBCブレーキタイプ選択ステアリングSW(Enter)操作制御目標値(TB_SLCT)	*/
#define TARGET_COM_DT_TB_SLCT_OFF			((uint8) 0U)		/*	ソフトSW未操作									*/
#define TARGET_COM_DT_TB_SLCT_ON			((uint8) 1U)		/*	ソフトSW操作									*/

/*	ドライブモニタ表示信号(瞬間燃費)制御目標値(IN_FC)	*/
/*	定義無し										*/

/*	ドライブモニタ表示信号(始動後平均車速)制御目標値(AS_SP)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(始動後走行距離)制御目標値(AS_DT)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(給油後平均燃費)制御目標値(AF_FC)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(航続可能距離)制御目標値(RANGE)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(通算平均燃費)制御目標値(TO_FC)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(始動後平均燃費)制御目標値(AS_FC)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(始動後走行時間)制御目標値(AS_TM)	*/
/*	定義無し											*/

/*	ドライブモニタ表示信号(通算走行時間) 出力値(TO_TM)	*/
/*	定義無し										*/

/*	メインタンクフューエル残量信号値制御目標値(FUEL_1)	*/
#define TARGET_COM_DT_FUEL1_UNKNOWN			((uint16) 65535U)	/* 未確定											*/

/*	メインタンクフューエル残量抵抗値制御目標値(FUEL_AD)	*/
#define TARGET_COM_DT_FUELAD_UNKNOWN		((uint16) 65535U)	/* 未確定											*/

/*	オド値(通信)制御目標値(ODO)	*/
#define	TARGET_COM_DT_ODTR_ODO_COM_INIT		((uint32) 0U)		/*	オド(通信)初期値								*/
#define	TARGET_COM_DT_ODTR_ODO_COM_MAX		((uint32) 999999U)	/*	オド(通信)最大値								*/

/*	トリップA(通信)制御目標値(TRIP_A)	*/
#define	TARGET_COM_DT_ODTR_TRIPA_COM_INIT	((uint32) 0U)		/*	トリップA(通信)初期値							*/
#define	TARGET_COM_DT_ODTR_TRIPA_COM_MAX	((uint32)99999U)	/*	トリップA(通信)最大値							*/

/*	トリップB(通信)制御目標値(TRIP_B)	*/
#define	TARGET_COM_DT_ODTR_TRIPB_COM_INIT	((uint32) 0U)		/*	トリップB(通信)初期値							*/
#define	TARGET_COM_DT_ODTR_TRIPB_COM_MAX	((uint32)99999U)	/*	トリップB(通信)最大値							*/

/*	Fuel平均値制御目標値	*/
#define	TARGET_COM_DT_FLAS_MIN				((uint8) 0U)		/*	Fuel平均値信号最小値							*/
#define	TARGET_COM_DT_FLAS_MAX				((uint8) 253U)		/*	Fuel平均値信号最大値							*/
#define	TARGET_COM_DT_FLAS_OPEN				((uint8) 254U)		/*	Fuel平均値信号センダオープン					*/
#define	TARGET_COM_DT_FLAS_UNKNOWN			((uint8) 255U)		/*	Fuel平均値信号データ未確定						*/
/* ↑ユーザー編集可能↑ */

#endif	/* BSW_COMMON_COMDEF_INC */


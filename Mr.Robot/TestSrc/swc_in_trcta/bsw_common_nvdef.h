/*	$RCSfile: bsw_common_nvdef.h $									*/
/*	$Date: 2015/11/04 20:36:59JST $									*/
/*	$Revision: 1.5 $												*/
/*	 EXPLANATION: BSW共通定義(不揮発性メモリ) 公開ヘッダファイル	*/

#ifndef BSW_COMMON_NVDEF_INC
#define BSW_COMMON_NVDEF_INC

#ifndef BSW_COMMON_NVDEF_DEF
#define BSW_COMMON_NVDEF_EXT extern
#else
#define BSW_COMMON_NVDEF_EXT
#endif

/*	TBC表示機能有無	*/
#define NV_TTBC_FUNC_STS_INVALID		((uint8) 0U)			/*	TBC表示機能無し						*/
#define NV_TTBC_FUNC_STS_VALID			((uint8) 1U)			/*	TBC表示機能有り						*/

/*	LDAシステム有無	*/
#define NV_KINOUM_LDA_OFF				((uint8) 0U)			/*	LDAシステム無し						*/
#define NV_KINOUM_LDA_ON				((uint8) 1U)			/*	LDAシステム有り						*/

/*	タイヤ空気圧システム有無	*/
#define NV_KINOUM_TPMS_OFF				((uint8) 0U)			/*	タイヤ空気圧システム無し			*/
#define NV_KINOUM_TPMS_ON				((uint8) 1U)			/*	タイヤ空気圧システム有り			*/

/*	シートベルトブザーキャンセル	*/
#define NV_BKLBZ_NONACTIVE				((uint8) 0U)			/*	シートベルトブザー機能NonActive		*/
#define NV_BKLBZ_ACTIVE					((uint8) 1U)			/*	シートベルトブザー機能Active		*/

/*	ソフトSW(RCTA)システム有無	*/
#define NV_KINOUM_RCTA_OFF				((uint8) 0U)			/*	ソフトSW(RCTA)システム無し			*/
#define NV_KINOUM_RCTA_ON				((uint8) 1U)			/*	ソフトSW(RCTA)システム有り			*/

/*	BSMシステム有無	*/
#define NV_KINOUM_BSM_OFF				((uint8) 0U)			/*	BSMシステム無し						*/
#define NV_KINOUM_BSM_ON				((uint8) 1U)			/*	BSMシステム有り						*/

/*	PCSシステム有無	*/
#define NV_KINOUM_PCS_OFF				((uint8) 0U)			/*	PCSシステム無し						*/
#define NV_KINOUM_PCS_ON				((uint8) 1U)			/*	PCSシステム有り						*/

/*	TPMS表示有無	*/
#define NV_TPMS_DISP_INVALID			((uint8) 0U)			/*	TPMS表示有り						*/
#define NV_TPMS_DISP_VALID				((uint8) 1U)			/*	TPMS表示無し						*/

/*	EHW2ウェイクアップ要因	*/
#define NV_WAKEUP_FACT_OFF				((uint8) 0U)			/*	要因無し							*/
#define NV_WAKEUP_FACT_ON				((uint8) 1U)			/*	要因有り							*/

/*	ハザード点灯メモリ状態	*/
#define NV_HZDMEM_OFF					((uint16) 0U)			/*	ハザードOFF							*/
#define NV_HZDMEM_ON					((uint16) 65535U)		/*	モーメンタリSW ON					*/
#define NV_HZDMEM_UNKNOWN				((uint16) 43690U)		/*	未定義								*/
#define NV_HZDMEM_RSCARFIND_ON			((uint16) 65533U)		/*	RSカーファインダ要求 ON				*/
#define NV_HZDMEM_RSHZD_ON				((uint16) 65534U)		/*	RSハザード要求 ON					*/

/*	ターンハザードショート・断線・DTC状態	*/
#define NV_TNHZD_LAMP_STS_NORMAL		((uint8) 0U)			/*	正常								*/
#define NV_TNHZD_LAMP_STS_SHORT			((uint8) 1U)			/*	ショート							*/
#define NV_TNHZD_LAMP_STS_BREAK			((uint8) 2U)			/*	断線								*/
#define NV_TNHZD_LAMP_STS_DTC_SHORT		((uint8) 4U)			/*	ショートDTC							*/
#define NV_TNHZD_LAMP_STS_DTC_BREAK		((uint8) 8U)			/*	断線DTC								*/

/*	比較データ状態	*/
#define NV_FUEL_DATSTS_NG				((uint8) 0U)
#define NV_FUEL_DATSTS_OK				((uint8) 1U)

/*	給油判定状態	*/
#define NV_FUEL_REFSTS_FALSE			((uint8) 0U)
#define NV_FUEL_REFSTS_TRUE				((uint8) 1U)

/*	VSC制御有無	*/
#define NV_KINOUM_VSC_OFF				((uint8) 0U)			/*	VSC制御無し							*/
#define NV_KINOUM_VSC_ON				((uint8) 1U)			/*	VSC制御有り							*/

/*	ブザー要求	*/
#define NV_BZ_OFF						((uint8) 0U)			/*	吹鳴解除							*/
#define NV_BZ_ON						((uint8) 1U)			/*	吹鳴登録							*/
#define NV_BZ_CYCLEOFF					((uint8) 2U)			/*	1周期後吹鳴解除						*/

/*	NV_ブザー状態	*/
#define NV_BZ_STATE_STOP				((uint8) 0U)			/*	ブザー状態：停止					*/
#define NV_BZ_STATE_WAIT				((uint8) 1U)			/*	ブザー状態：吹鳴待ち				*/
#define NV_BZ_STATE_RUN					((uint8) 2U)			/*	ブザー状態：吹鳴					*/
#define NV_BZ_STATE_FINISHED			((uint8) 3U)			/*	ブザー状態：完了					*/
#define NV_BZ_STATE_CYCLESTOP			((uint8) 4U)			/*	ブザー状態：1周期後OFF				*/

/*	調光ステップ値(非減光)	*/
#define NV_ILLCNT_STEP1D				((uint8) 0U)			/*	非減光モードSTEP1					*/
#define NV_ILLCNT_STEP2D				((uint8) 1U)			/*	非減光モードSTEP2					*/
#define NV_ILLCNT_STEP3D				((uint8) 2U)			/*	非減光モードSTEP3					*/
#define NV_ILLCNT_STEP4D				((uint8) 3U)			/*	非減光モードSTEP4					*/
#define NV_ILLCNT_STEP5D				((uint8) 4U)			/*	非減光モードSTEP5					*/
#define NV_ILLCNT_STEP6D				((uint8) 5U)			/*	非減光モードSTEP6					*/
#define NV_ILLCNT_STEP7D				((uint8) 6U)			/*	非減光モードSTEP7					*/
#define NV_ILLCNT_STEP8D				((uint8) 7U)			/*	非減光モードSTEP8					*/
#define NV_ILLCNT_STEP9D				((uint8) 8U)			/*	非減光モードSTEP9					*/
#define NV_ILLCNT_STEP10D				((uint8) 9U)			/*	非減光モードSTEP10					*/
#define NV_ILLCNT_STEP11D				((uint8) 10U)			/*	非減光モードSTEP11					*/
#define NV_ILLCNT_STEP12D				((uint8) 11U)			/*	非減光モードSTEP12					*/
#define NV_ILLCNT_STEP13D				((uint8) 12U)			/*	非減光モードSTEP13					*/
#define NV_ILLCNT_STEP14D				((uint8) 13U)			/*	非減光モードSTEP14					*/
#define NV_ILLCNT_STEP15D				((uint8) 14U)			/*	非減光モードSTEP15					*/
#define NV_ILLCNT_STEP16D				((uint8) 15U)			/*	非減光モードSTEP16					*/
#define NV_ILLCNT_STEP17D				((uint8) 16U)			/*	非減光モードSTEP17					*/
#define NV_ILLCNT_STEP18D				((uint8) 17U)			/*	非減光モードSTEP18					*/
#define NV_ILLCNT_STEP19D				((uint8) 18U)			/*	非減光モードSTEP19					*/
#define NV_ILLCNT_STEP20D				((uint8) 19U)			/*	非減光モードSTEP20					*/

/*	調光ステップ値(減光)	*/
#define NV_ILLCNT_STEP1N				((uint8) 0U)			/*	減光モードSTEP1						*/
#define NV_ILLCNT_STEP2N				((uint8) 1U)			/*	減光モードSTEP2						*/
#define NV_ILLCNT_STEP3N				((uint8) 2U)			/*	減光モードSTEP3						*/
#define NV_ILLCNT_STEP4N				((uint8) 3U)			/*	減光モードSTEP4						*/
#define NV_ILLCNT_STEP5N				((uint8) 4U)			/*	減光モードSTEP5						*/
#define NV_ILLCNT_STEP6N				((uint8) 5U)			/*	減光モードSTEP6						*/
#define NV_ILLCNT_STEP7N				((uint8) 6U)			/*	減光モードSTEP7						*/
#define NV_ILLCNT_STEP8N				((uint8) 7U)			/*	減光モードSTEP8						*/
#define NV_ILLCNT_STEP9N				((uint8) 8U)			/*	減光モードSTEP9						*/
#define NV_ILLCNT_STEP10N				((uint8) 9U)			/*	減光モードSTEP10					*/
#define NV_ILLCNT_STEP11N				((uint8) 10U)			/*	減光モードSTEP11					*/
#define NV_ILLCNT_STEP12N				((uint8) 11U)			/*	減光モードSTEP12					*/
#define NV_ILLCNT_STEP13N				((uint8) 12U)			/*	減光モードSTEP13					*/
#define NV_ILLCNT_STEP14N				((uint8) 13U)			/*	減光モードSTEP14					*/
#define NV_ILLCNT_STEP15N				((uint8) 14U)			/*	減光モードSTEP15					*/
#define NV_ILLCNT_STEP16N				((uint8) 15U)			/*	減光モードSTEP16					*/
#define NV_ILLCNT_STEP17N				((uint8) 16U)			/*	減光モードSTEP17					*/
#define NV_ILLCNT_STEP18N				((uint8) 17U)			/*	減光モードSTEP18					*/
#define NV_ILLCNT_STEP19N				((uint8) 18U)			/*	減光モードSTEP19					*/
#define NV_ILLCNT_STEP20N				((uint8) 19U)			/*	減光モードSTEP20					*/

/*	ラウンジイルミ消灯信号	*/
#define NV_ILLOUT_ILLOF_ON				((uint8) 0U)			/*	点灯指示							*/
#define NV_ILLOUT_ILLOF_OFF				((uint8) 1U)			/*	消灯指示							*/

/*	NVオドトリ表示種別	*/
#define	NV_ODTR_DISPSTS_ODO				((uint8) 0U)			/*	ODO表示								*/
#define	NV_ODTR_DISPSTS_TRIPA			((uint8) 1U)			/*	TRIPA表示							*/
#define	NV_ODTR_DISPSTS_TRIPB			((uint8) 2U)			/*	TRIPB表示							*/

/*	NVオド積算パルス	*/
#define	NV_ODO_PULSE_MIN				((uint16) 0U)			/*	最小オド積算パルス					*/
#define	NV_ODO_PULSE_MAX				((uint16) 65535U)		/*	最大オド積算パルス					*/

/*	NVトリップA積算パルス	*/
#define	NV_TRIPA_PULSE_MIN				((uint16) 0U)			/*	最小トリップA積算パルス				*/
#define	NV_TRIPA_PULSE_MAX				((uint16) 65535U)		/*	最大トリップA積算パルス				*/

/*	NVトリップB積算パルス	*/
#define	NV_TRIPB_PULSE_MIN				((uint16) 0U)			/*	最小トリップB積算パルス				*/
#define	NV_TRIPB_PULSE_MAX				((uint16) 65535U)		/*	最大トリップB積算パルス				*/

/*	NVオド単位情報	*/
#define	NV_ODO_UNIT_KM					((uint8) 0U)			/*	オド単位：KM						*/
#define	NV_ODO_UNIT_MILE				((uint8) 1U)			/*	オド単位：MILE						*/

/*	NVオド表示値単位情報	*/
#define	NV_ODO_DISP_UNIT_KM				((uint8) 0U)			/*	オド表示値単位：KM					*/
#define	NV_ODO_DISP_UNIT_MILE			((uint8) 1U)			/*	オド表示値単位：MILE				*/

/*	NV端数パルス整合性チェック	*/
#define	NV_CHKPLS_MIN					((uint16) 0U)			/*	端数パルス最小値					*/
#define	NV_CHKPLS_MAX					((uint16) 5095U)		/*	端数パルス最大値					*/
#define	NV_CHKPLS_FALSE					((uint16) 0xAA55U)		/*	範囲外データ						*/

/*	ドライブモニタ単位切替情報	*/
#define	NV_MCUST_UNIT_NOT_SET			((uint8) 0U)			/*	未設定								*/
#define	NV_MCUST_UNIT_KML				((uint8) 1U)			/*	km/L								*/
#define	NV_MCUST_UNIT_L100KM			((uint8) 2U)			/*	L/100km								*/
#define	NV_MCUST_UNIT_MPG_US			((uint8) 3U)			/*	MPG(US)								*/
#define	NV_MCUST_UNIT_MPG_UK			((uint8) 4U)			/*	MPG(UK)								*/
#define	NV_MCUST_UNIT_KMKG				((uint8) 5U)			/*	km/kg								*/
#define	NV_MCUST_UNIT_KMGALLON			((uint8) 6U)			/*	km/gallon							*/

/*	NVオド値					*/
#define	NV_ODO_MIN						((uint32) 0U)			/*	最小オド値							*/
#define	NV_ODO_MAX						((uint32) 999999U)		/*	最大オド値							*/

/*	NVトリップ値A				*/
#define	NV_TRIPA_MIN					((uint32) 0U)			/*	最小トリップA値						*/
#define	NV_TRIPA_MAX					((uint32) 99999U)		/*	最大トリップA値						*/

/*	NVトリップ値B				*/
#define	NV_TRIPB_MIN					((uint32) 0U)			/*	最小トリップB値						*/
#define	NV_TRIPB_MAX					((uint32) 99999U)		/*	最大トリップB値						*/

#endif	/* BSW_COMMON_NVDEF_INC */


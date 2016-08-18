/*	$RCSfile: Swc_custom.h $						*/
/*	$Date: 2015/xx/xx 12:00:00JST $					*/
/*	$Revision: 1.1 $								*/
/*	 EXPLANATION: Swc_custom.hダミーヘッダファイル	*/

#ifndef SWC_CUSTOM_INC
#define SWC_CUSTOM_INC

/****************************************************/
/* 下記をビルド対象のSWC名称で書き換える必要あり	*/
/****************************************************/
#define	swc_in_tslpoff_CODE
#define	swc_prc_tslpoff_CODE
#define	swc_in_analog_CODE
#define	swc_cd_sound_CODE
#define	swc_in_oilp_CODE
/***************************************************/

/* Std_ReturnTypeの戻り値定義	*/
/* ⇒必要に応じて追加すること	*/
#define	RTE_E_OK			((uint8) 0)

#endif	/*	SWC_CUSTOM_INC	*/


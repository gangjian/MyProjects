/*
 * :AMSTB_SrcFile.c
 * : BSW単体テスト用
 */
#define WINAMS_STUB
#ifdef WINAMS_STUB
#ifdef __cplusplus
extern "C" {
#endif
#include "Platform_Types.h"


/*------------------------------------------------------------------------------*/
/*   スタブ関数                                                                 */
/*------------------------------------------------------------------------------*/
/* g01 */
static uint8 stb_makeEngOnOff3s_Cnt;
void AMSTB_makeEngOnOff3s(void){
	stb_makeEngOnOff3s_Cnt++;
}

/* g01 */
static uint8 stb_makeIgv_Cnt;
void AMSTB_makeIgv(void){
	stb_makeIgv_Cnt++;
}

/* g01 */
static uint8 stb_makeOilpAd_Cnt;
void AMSTB_makeOilpAd(void){
	stb_makeOilpAd_Cnt++;
}

/* g02, g03 */
static uint8 stb_initPvOilp_Cnt;
void AMSTB_initPvOilp(void){
	stb_initPvOilp_Cnt++;
}

/* s03 */
static uint8  stb_makeNe1Signed_Cnt;
static uint16 stb_makeNe1Signed_Ret;
uint16 AMSTB_makeNe1Signed(void){
	stb_makeNe1Signed_Cnt++;
	return stb_makeNe1Signed_Ret;
}

/* s04 */
static uint8 stb_makeEngRevFail3s_Cnt;
static uint16 stb_makeEngRevFail3s_engRev3s;
static uint8 stb_makeEngRevFail3s_engRevSts3s;
void AMSTB_makeEngRevFail3s(uint16 *engRev3s, uint8 *engRevSts3s){
	stb_makeEngRevFail3s_Cnt++;
	*engRev3s = stb_makeEngRevFail3s_engRev3s;
	*engRevSts3s = stb_makeEngRevFail3s_engRevSts3s;
}

/* s06 */
static uint8 stb_jdgSampSts_Cnt;
static uint8 stb_jdgSampSts_Ret;
uint8 AMSTB_jdgSampSts(void)
{
	stb_jdgSampSts_Cnt++;
	return stb_jdgSampSts_Ret;
}
/* s06 */
static uint8 stb_calcIgAdAvrgData_Cnt;
static uint16 stb_calcIgAdAvrgData_Ret;
static uint8 stb_calcIgAdAvrgData_sampSts;
uint16 AMSTB_calcIgAdAvrgData(uint8 sampSts)
{
	stb_calcIgAdAvrgData_Cnt++;
	stb_calcIgAdAvrgData_sampSts = sampSts;
	return stb_calcIgAdAvrgData_Ret;
}
/* s06 */
static uint8 stb_correctOilpAd_Cnt;
static uint16 stb_correctOilpAd_Ret;
static uint16 stb_correctOilpAd_igAdAvrg;
static uint16 stb_correctOilpAd_oilpAd;
uint16 AMSTB_correctOilpAd(uint16 igAdAvrg, uint16 oilpAd)
{
	stb_correctOilpAd_Cnt++;
	stb_correctOilpAd_igAdAvrg = igAdAvrg;
	stb_correctOilpAd_oilpAd = oilpAd;
	return stb_correctOilpAd_Ret;
}






#ifdef __cplusplus
}
#endif
#endif /* WINAMS_STUB */

package com.impinker.impinkerai.handle;

import com.impinker.impinkerai.enums.ResultEnum;
import com.impinker.impinkerai.exception.SearchException;
import com.impinker.impinkerai.util.ResultUtil;
import com.impinker.impinkerai.vo.ResultVo;
import lombok.extern.slf4j.Slf4j;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.ResponseBody;

@ControllerAdvice
@Slf4j
public class SearchExceptionHandle {

    @ExceptionHandler(value = Exception.class)
    @ResponseBody
    public ResultVo handle(Exception e) {
        if (e instanceof SearchException) {
            SearchException searchException = (SearchException) e;
            return ResultUtil.error(searchException.getResultEnum());
        } else {
            log.error("【系统异常】{}", e);
            return ResultUtil.error(ResultEnum.UNKNOWN_ERROR);
        }
    }
}

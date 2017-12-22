package com.impinker.impinkerai.exception;

import com.impinker.impinkerai.enums.ResultEnum;
import lombok.Data;
import lombok.Getter;
import lombok.Setter;

/**
 * 异常处理
 */
public class SearchException extends RuntimeException {

    @Getter
    @Setter
    private Integer code;

    @Getter
    @Setter
    private ResultEnum resultEnum;

    public SearchException(ResultEnum resultEnum) {
        super(resultEnum.getMessage());
        this.code = resultEnum.getCode();
        this.resultEnum=resultEnum;
    }

    public SearchException(Integer code, String message) {
        super(message);
        this.code = code;
    }

}
